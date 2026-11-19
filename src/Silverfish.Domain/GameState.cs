using System.Collections.Immutable;

namespace Silverfish.Domain;

public readonly record struct PlayerId(int Value);

public enum GameZone
{
    Deck, Hand, Play, Graveyard, Secret, SetAside, Removed, HeroPower
}

public readonly record struct EntityLocation(PlayerId Player, GameZone Zone);

public sealed record PlayerState(PlayerId Id, EntityId Hero, int Mana = 0, int ManaCrystals = 0, int Fatigue = 0)
{
    public EntityId? EquippedWeapon { get; init; }
    public int HeroPowerUsesThisTurn { get; init; }
    public int OverloadOwed { get; init; }
    public int OverloadLocked { get; init; }
    public ImmutableDictionary<GameZone, ImmutableArray<EntityId>> Zones { get; init; } =
        ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty;

    public ImmutableArray<EntityId> In(GameZone zone) =>
        Zones.TryGetValue(zone, out var entities) ? entities : ImmutableArray<EntityId>.Empty;
}

public sealed record CharacterState(int Attack, int MaximumHealth, int Damage = 0, int Armor = 0,
    int AttacksThisTurn = 0, bool Exhausted = false, bool DivineShield = false)
{
    public int? BaseAttack { get; init; }
    public int? BaseMaximumHealth { get; init; }
    public bool Frozen { get; init; }
    public int Health => MaximumHealth - Damage;
}

public sealed record GameEntity(EntityId Id, string CardId, PlayerId Owner, PlayerId Controller)
{
    public CharacterState? Character { get; init; }
    public WeaponStats? Weapon { get; init; }
    public long? PlayOrder { get; init; }
    public bool Silenced { get; init; }
    public ImmutableArray<StatEnchantment> Enchantments { get; init; } = [];
    public ImmutableArray<StatAuraGrant> AuraGrants { get; init; } = [];
    public ImmutableArray<ManaCostModifier> CostModifiers { get; init; } = [];
}

public enum MatchOutcome { InProgress, Draw, Won }

/// <summary>Immutable simulator state. Ordered zones index the canonical entity table.</summary>
public sealed class GameState
{
    private readonly ImmutableDictionary<EntityId, EntityLocation> _locations;

    private GameState(ImmutableDictionary<PlayerId, PlayerState> players,
        ImmutableDictionary<EntityId, GameEntity> entities,
        ImmutableDictionary<EntityId, EntityLocation> locations, PlayerId activePlayer,
        int turn, int nextEntity, long nextPlayOrder, MatchOutcome outcome, PlayerId? winner,
        ImmutableList<MinionDeath> minionDeaths, ulong randomState)
    {
        Players = players;
        Entities = entities;
        _locations = locations;
        ActivePlayer = activePlayer;
        Turn = turn;
        NextEntity = nextEntity;
        NextPlayOrder = nextPlayOrder;
        Outcome = outcome;
        Winner = winner;
        MinionDeaths = minionDeaths;
        RandomState = randomState;
    }

    public ImmutableDictionary<PlayerId, PlayerState> Players { get; }
    public ImmutableDictionary<EntityId, GameEntity> Entities { get; }
    public PlayerId ActivePlayer { get; }
    public int Turn { get; }
    public MatchOutcome Outcome { get; }
    public PlayerId? Winner { get; }
    public int NextEntity { get; }
    public long NextPlayOrder { get; }
    public ImmutableList<MinionDeath> MinionDeaths { get; }
    public ulong RandomState { get; }

    internal static GameState Create(IEnumerable<PlayerState> players, IEnumerable<GameEntity> entities,
        PlayerId activePlayer, int turn, IEnumerable<MinionDeath>? minionDeaths, ulong randomState)
    {
        var byPlayer = players.ToImmutableDictionary(p => (p ?? throw new ArgumentException("Null player.")).Id);
        var byEntity = entities.ToImmutableDictionary(e => (e ?? throw new ArgumentException("Null entity.")).Id);
        if (byPlayer.Count != 2 || !byPlayer.ContainsKey(activePlayer) || turn < 1)
            throw new ArgumentException("A match needs two players, an active player, and a positive turn number.");

        var locations = ImmutableDictionary.CreateBuilder<EntityId, EntityLocation>();
        var playOrders = new HashSet<long>();
        foreach (var player in byPlayer.Values)
        {
            ValidateResources(player);
            if (!player.In(GameZone.Play).Contains(player.Hero) ||
                !byEntity.TryGetValue(player.Hero, out var hero) || hero.Character is null)
                throw new ArgumentException($"Player {player.Id.Value} must have a hero in play.");
            if (player.EquippedWeapon is EntityId equipped && (!player.In(GameZone.Play).Contains(equipped) ||
                !byEntity.TryGetValue(equipped, out var weapon) || weapon.Weapon is not { Durability: > 0 }))
                throw new ArgumentException($"Player {player.Id.Value} has an invalid equipped weapon.");
            if (player.In(GameZone.HeroPower).Length > 1 ||
                (player.In(GameZone.HeroPower).IsEmpty && player.HeroPowerUsesThisTurn != 0))
                throw new ArgumentException($"Player {player.Id.Value} has an invalid hero power slot.");

            foreach (var zone in player.Zones)
            {
                if (!Enum.IsDefined(zone.Key) || zone.Value.IsDefault)
                    throw new ArgumentException("Each zone must be initialized and have a known kind.");

                foreach (var id in zone.Value)
                {
                    if (!byEntity.TryGetValue(id, out var entity))
                        throw new ArgumentException($"Zone references absent entity {id}.");
                    ValidateEntity(entity, byPlayer);
                    if (entity.Controller != player.Id || !locations.TryAdd(id, new(player.Id, zone.Key)))
                        throw new ArgumentException($"Entity {id} needs exactly one zone for its controller.");

                    if (zone.Key == GameZone.Play && (entity.Character is not null || entity.Weapon is not null) &&
                        (entity.PlayOrder is not > 0 || !playOrders.Add(entity.PlayOrder.Value)))
                        throw new ArgumentException($"In-play entity {id} needs a unique positive entry order.");
                }
            }
        }
        if (locations.Count != byEntity.Count)
            throw new ArgumentException("Every registered entity must have a zone.");

        var deaths = minionDeaths?.ToImmutableList() ?? ImmutableList<MinionDeath>.Empty;
        var deathTurn = 1;
        foreach (var death in deaths)
        {
            if (death is null || death.Turn < deathTurn || death.Turn > turn || string.IsNullOrWhiteSpace(death.CardId) ||
                !byEntity.ContainsKey(death.Entity) || !byPlayer.ContainsKey(death.Controller))
                throw new ArgumentException("Invalid minion death history.", nameof(minionDeaths));
            deathTurn = death.Turn;
        }

        return new GameState(byPlayer, byEntity, locations.ToImmutable(), activePlayer, turn,
            checked(byEntity.Keys.Max(id => id.Value) + 1),
            checked(byEntity.Values.Max(e => e.PlayOrder ?? 0) + 1), MatchOutcome.InProgress, null, deaths, randomState);
    }

    public EntityLocation LocationOf(EntityId id) => _locations[id];

    public PlayerId OpponentOf(PlayerId player)
    {
        if (!Players.ContainsKey(player))
            throw new KeyNotFoundException($"Unknown player {player.Value}.");
        return Players.Keys.Single(id => id != player);
    }

    // Structural edits are shared by all rules. Ownership is preserved on a control change.
    internal GameState Move(EntityId id, EntityLocation destination, int? index = null)
    {
        var origin = LocationOf(id);
        var entity = Entities[id];
        if (Players[origin.Player].Hero == id && destination.Player != origin.Player)
            throw new InvalidOperationException("The hero slot cannot be transferred as a card.");

        var sourcePlayer = Players[origin.Player];
        if (sourcePlayer.EquippedWeapon == id && origin != destination)
            sourcePlayer = sourcePlayer with { EquippedWeapon = null };
        var players = Players.SetItem(sourcePlayer.Id, SetZone(sourcePlayer, origin.Zone, sourcePlayer.In(origin.Zone).Remove(id)));
        var to = players[destination.Player];
        var zone = to.In(destination.Zone);
        to = SetZone(to, destination.Zone, zone.Insert(index ?? zone.Length, id));
        var entersPlay = destination.Zone == GameZone.Play && origin.Zone != GameZone.Play;
        entity = entity with
        {
            Controller = destination.Player,
            PlayOrder = entersPlay ? NextPlayOrder : entity.PlayOrder,
            AuraGrants = destination.Zone == GameZone.Play ? entity.AuraGrants : [],
            Enchantments = destination.Zone == GameZone.Play ? entity.Enchantments : [],
            CostModifiers = destination.Zone == GameZone.Hand ? entity.CostModifiers : []
        };
        return new GameState(players.SetItem(to.Id, to), Entities.SetItem(id, entity), _locations.SetItem(id, destination),
            ActivePlayer, Turn, NextEntity, entersPlay ? checked(NextPlayOrder + 1) : NextPlayOrder, Outcome, Winner, MinionDeaths, RandomState);
    }

    internal GameState UpdateCharacter(EntityId id, CharacterState character) =>
        UpdateEntity(Entities[id] with { Character = character });

    internal GameState UpdateEntity(GameEntity entity)
    {
        if (!Entities.TryGetValue(entity.Id, out var current) || entity.Owner != current.Owner ||
            entity.Controller != current.Controller || entity.PlayOrder != current.PlayOrder)
            throw new ArgumentException("Entity identity and location can only change through structural edits.", nameof(entity));
        ValidateEntity(entity, Players);
        return new GameState(Players, Entities.SetItem(entity.Id, entity), _locations,
            ActivePlayer, Turn, NextEntity, NextPlayOrder, Outcome, Winner, MinionDeaths, RandomState);
    }

    internal GameState AddToPlay(GameEntity entity, int index)
    {
        ValidateEntity(entity, Players);
        if (entity.Id.Value != NextEntity || (entity.Character is null && entity.Weapon is null))
            throw new ArgumentException("A new character or weapon needs the next unused entity ID.");
        var player = Players[entity.Controller];
        player = SetZone(player, GameZone.Play, player.In(GameZone.Play).Insert(index, entity.Id));
        return new GameState(Players.SetItem(player.Id, player),
            Entities.Add(entity.Id, entity with { PlayOrder = NextPlayOrder }),
            _locations.Add(entity.Id, new(player.Id, GameZone.Play)), ActivePlayer, Turn,
            checked(NextEntity + 1), checked(NextPlayOrder + 1), Outcome, Winner, MinionDeaths, RandomState);
    }

    internal GameState SetResources(PlayerId id, int mana, int manaCrystals, int fatigue)
    {
        var player = Players[id] with { Mana = mana, ManaCrystals = manaCrystals, Fatigue = fatigue };
        ValidateResources(player);
        return UpdatePlayer(player);
    }

    internal GameState EquipWeapon(PlayerId player, EntityId id)
    {
        if (LocationOf(id) != new EntityLocation(player, GameZone.Play) || Entities[id].Weapon is not { Durability: > 0 })
            throw new ArgumentException("An equipped weapon needs positive durability and must be in play.", nameof(id));
        return UpdatePlayer(Players[player] with { EquippedWeapon = id });
    }

    internal GameState SetHeroPowerUses(PlayerId player, int uses) =>
        UpdatePlayer(Players[player] with { HeroPowerUsesThisTurn = uses });

    internal GameState SetMana(PlayerId id, int mana, int crystals, int overloadOwed, int overloadLocked)
    {
        var player = Players[id] with
        {
            Mana = mana, ManaCrystals = crystals, OverloadOwed = overloadOwed, OverloadLocked = overloadLocked
        };
        ValidateResources(player);
        return UpdatePlayer(player);
    }

    private GameState UpdatePlayer(PlayerState player) =>
        new(Players.SetItem(player.Id, player), Entities, _locations,
            ActivePlayer, Turn, NextEntity, NextPlayOrder, Outcome, Winner, MinionDeaths, RandomState);

    internal GameState RecordMinionDeath(GameEntity minion) => new(Players, Entities, _locations, ActivePlayer, Turn,
        NextEntity, NextPlayOrder, Outcome, Winner, MinionDeaths.Add(new(minion.Id, minion.CardId, minion.Controller, Turn)), RandomState);

    internal GameState SetRandomState(ulong randomState) => new(Players, Entities, _locations, ActivePlayer, Turn,
        NextEntity, NextPlayOrder, Outcome, Winner, MinionDeaths, randomState);

    internal GameState PassTurn() => new(Players, Entities, _locations, OpponentOf(ActivePlayer),
        checked(Turn + 1), NextEntity, NextPlayOrder, Outcome, Winner, MinionDeaths, RandomState);

    internal GameState Finish(PlayerId? winner)
    {
        if (winner is PlayerId id && !Players.ContainsKey(id))
            throw new ArgumentException("The winner must be a player in this match.", nameof(winner));
        return new GameState(Players, Entities, _locations, ActivePlayer, Turn, NextEntity, NextPlayOrder,
            winner is null ? MatchOutcome.Draw : MatchOutcome.Won, winner, MinionDeaths, RandomState);
    }

    private static PlayerState SetZone(PlayerState player, GameZone zone, ImmutableArray<EntityId> contents)
    {
        if (!Enum.IsDefined(zone))
            throw new ArgumentOutOfRangeException(nameof(zone));
        return player with { Zones = player.Zones.SetItem(zone, contents) };
    }

    private static void ValidateResources(PlayerState player)
    {
        if (player.Zones is null || player.Id.Value < 1 || player.Mana < 0 || player.ManaCrystals is < 0 or > 10 ||
            player.Mana > 10 || player.Fatigue < 0 || player.HeroPowerUsesThisTurn is < 0 or > 1 ||
            player.OverloadLocked < 0 || player.OverloadOwed < 0)
            throw new ArgumentException("Invalid player ID or resource values.");
    }

    private static void ValidateEntity(GameEntity entity, IReadOnlyDictionary<PlayerId, PlayerState> players)
    {
        if (entity.Id.Value < 0 || string.IsNullOrWhiteSpace(entity.CardId) ||
            !players.ContainsKey(entity.Owner) || !players.ContainsKey(entity.Controller) || entity.PlayOrder is < 1 ||
            entity.Enchantments.IsDefault || entity.AuraGrants.IsDefault || entity.CostModifiers.IsDefault)
            throw new ArgumentException("Invalid entity identity, owner, controller, or entry order.");
        if (entity.Character is { } character)
            ValidateCharacter(character);
        if (entity.Weapon is { } weapon && (entity.Character is not null || weapon.Attack < 0 || weapon.Durability < 0 ||
            weapon.MaximumDurability < 1 || weapon.Durability > weapon.MaximumDurability || weapon.AttacksThisTurn < 0))
            throw new ArgumentException("Invalid weapon stats.");
    }

    private static void ValidateCharacter(CharacterState character)
    {
        if (character.Attack < 0 || character.MaximumHealth < 1 || character.Damage < 0 ||
            character.Armor < 0 || character.AttacksThisTurn < 0 ||
            character.BaseAttack is < 0 || character.BaseMaximumHealth is < 1)
            throw new ArgumentException("Invalid character stats.");
    }
}
