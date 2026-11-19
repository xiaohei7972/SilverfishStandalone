using System.Collections.Immutable;

namespace Silverfish.Domain;

public readonly record struct PlayerId(int Value);

public enum GameZone
{
    Deck, Hand, Play, Graveyard, Secret, SetAside, Removed
}

public readonly record struct EntityLocation(PlayerId Player, GameZone Zone);

public sealed record PlayerState(PlayerId Id, EntityId Hero, int Mana = 0, int ManaCrystals = 0, int Fatigue = 0)
{
    public ImmutableDictionary<GameZone, ImmutableArray<EntityId>> Zones { get; init; } =
        ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty;

    public ImmutableArray<EntityId> In(GameZone zone) =>
        Zones.TryGetValue(zone, out var entities) ? entities : ImmutableArray<EntityId>.Empty;
}

public sealed record CharacterState(int Attack, int MaximumHealth, int Damage = 0, int Armor = 0,
    int AttacksThisTurn = 0, bool Exhausted = false, bool DivineShield = false)
{
    public int Health => MaximumHealth - Damage;
}

public sealed record GameEntity(EntityId Id, string CardId, PlayerId Owner, PlayerId Controller)
{
    public CharacterState? Character { get; init; }
    public long? PlayOrder { get; init; }
}

public enum MatchOutcome { InProgress, Draw, Won }

/// <summary>Immutable simulator state. Ordered zones index the canonical entity table.</summary>
public sealed class GameState
{
    private readonly ImmutableDictionary<EntityId, EntityLocation> _locations;

    private GameState(ImmutableDictionary<PlayerId, PlayerState> players,
        ImmutableDictionary<EntityId, GameEntity> entities,
        ImmutableDictionary<EntityId, EntityLocation> locations, PlayerId activePlayer,
        int turn, int nextEntity, long nextPlayOrder, MatchOutcome outcome, PlayerId? winner)
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
    }

    public ImmutableDictionary<PlayerId, PlayerState> Players { get; }
    public ImmutableDictionary<EntityId, GameEntity> Entities { get; }
    public PlayerId ActivePlayer { get; }
    public int Turn { get; }
    public MatchOutcome Outcome { get; }
    public PlayerId? Winner { get; }
    public int NextEntity { get; }
    public long NextPlayOrder { get; }

    internal static GameState Create(IEnumerable<PlayerState> players, IEnumerable<GameEntity> entities,
        PlayerId activePlayer, int turn = 1)
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

                    if (zone.Key == GameZone.Play && entity.Character is not null &&
                        (entity.PlayOrder is not > 0 || !playOrders.Add(entity.PlayOrder.Value)))
                        throw new ArgumentException($"In-play character {id} needs a unique positive entry order.");
                }
            }
        }
        if (locations.Count != byEntity.Count)
            throw new ArgumentException("Every registered entity must have a zone.");

        return new GameState(byPlayer, byEntity, locations.ToImmutable(), activePlayer, turn,
            checked(byEntity.Keys.Max(id => id.Value) + 1),
            checked(byEntity.Values.Max(e => e.PlayOrder ?? 0) + 1), MatchOutcome.InProgress, null);
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

        var from = Players[origin.Player];
        var players = Players.SetItem(from.Id, SetZone(from, origin.Zone, from.In(origin.Zone).Remove(id)));
        var to = players[destination.Player];
        var zone = to.In(destination.Zone);
        to = SetZone(to, destination.Zone, zone.Insert(index ?? zone.Length, id));
        var entersPlay = destination.Zone == GameZone.Play && origin.Zone != GameZone.Play;
        entity = entity with
        {
            Controller = destination.Player,
            PlayOrder = entersPlay ? NextPlayOrder : entity.PlayOrder
        };
        return new GameState(players.SetItem(to.Id, to), Entities.SetItem(id, entity), _locations.SetItem(id, destination),
            ActivePlayer, Turn, NextEntity, entersPlay ? checked(NextPlayOrder + 1) : NextPlayOrder, Outcome, Winner);
    }

    internal GameState UpdateCharacter(EntityId id, CharacterState character)
    {
        ValidateCharacter(character);
        return new GameState(Players, Entities.SetItem(id, Entities[id] with { Character = character }), _locations,
            ActivePlayer, Turn, NextEntity, NextPlayOrder, Outcome, Winner);
    }

    internal GameState AddToPlay(GameEntity entity, int index)
    {
        ValidateEntity(entity, Players);
        if (entity.Id.Value != NextEntity || entity.Character is null)
            throw new ArgumentException("A new character needs the next unused entity ID.");
        var player = Players[entity.Controller];
        player = SetZone(player, GameZone.Play, player.In(GameZone.Play).Insert(index, entity.Id));
        return new GameState(Players.SetItem(player.Id, player),
            Entities.Add(entity.Id, entity with { PlayOrder = NextPlayOrder }),
            _locations.Add(entity.Id, new(player.Id, GameZone.Play)), ActivePlayer, Turn,
            checked(NextEntity + 1), checked(NextPlayOrder + 1), Outcome, Winner);
    }

    internal GameState SetResources(PlayerId id, int mana, int manaCrystals, int fatigue)
    {
        var player = Players[id] with { Mana = mana, ManaCrystals = manaCrystals, Fatigue = fatigue };
        ValidateResources(player);
        return new GameState(Players.SetItem(id, player), Entities, _locations,
            ActivePlayer, Turn, NextEntity, NextPlayOrder, Outcome, Winner);
    }

    internal GameState PassTurn() => new(Players, Entities, _locations, OpponentOf(ActivePlayer),
        checked(Turn + 1), NextEntity, NextPlayOrder, Outcome, Winner);

    internal GameState Finish(PlayerId? winner)
    {
        if (winner is PlayerId id && !Players.ContainsKey(id))
            throw new ArgumentException("The winner must be a player in this match.", nameof(winner));
        return new GameState(Players, Entities, _locations, ActivePlayer, Turn, NextEntity, NextPlayOrder,
            winner is null ? MatchOutcome.Draw : MatchOutcome.Won, winner);
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
            player.Mana > 10 || player.Fatigue < 0)
            throw new ArgumentException("Invalid player ID or resource values.");
    }

    private static void ValidateEntity(GameEntity entity, IReadOnlyDictionary<PlayerId, PlayerState> players)
    {
        if (entity.Id.Value < 0 || string.IsNullOrWhiteSpace(entity.CardId) ||
            !players.ContainsKey(entity.Owner) || !players.ContainsKey(entity.Controller) || entity.PlayOrder is < 1)
            throw new ArgumentException("Invalid entity identity, owner, controller, or entry order.");
        if (entity.Character is { } character)
            ValidateCharacter(character);
    }

    private static void ValidateCharacter(CharacterState character)
    {
        if (character.Attack < 0 || character.MaximumHealth < 1 || character.Damage < 0 ||
            character.Armor < 0 || character.AttacksThisTurn < 0)
            throw new ArgumentException("Invalid character stats.");
    }
}
