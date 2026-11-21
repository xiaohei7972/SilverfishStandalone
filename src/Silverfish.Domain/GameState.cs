using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Silverfish.Domain;

public readonly record struct PlayerId(int Value);

public enum GameZone
{
    Deck, Hand, Play, Graveyard, Secret, SetAside, Removed, HeroPower
}

public readonly record struct EntityLocation(PlayerId Player, GameZone Zone);

public sealed record PlayerState(PlayerId Id, EntityId Hero, int Mana = 0, int ManaCrystals = 0, int Fatigue = 0)
{
    private readonly int _initialMana = Mana;

    public int Mana
    {
        get => ManaSpent is int spent
            ? (int)Math.Clamp((long)ManaCrystals + TemporaryMana - OverloadLocked - spent, 0, int.MaxValue)
            : _initialMana;
        init => _initialMana = value;
    }

    public int? ManaSpent { get; init; }
    public int TemporaryMana { get; init; }
    public int MaximumManaCrystals { get; init; } = 10;
    public EntityId? EquippedWeapon { get; init; }
    public int HeroPowerUsesThisTurn { get; init; }
    public int OverloadOwed { get; init; }
    public int OverloadLocked { get; init; }
    public bool? PlayedCardThisTurn { get; init; }
    public bool? PlayedElementalThisTurn { get; init; }
    public bool? PlayedElementalLastTurn { get; init; }
    public bool? HeroHealthChangedThisTurn { get; init; }
    public bool SteadyShotCanTargetMinions { get; init; }
    public int? Corpses { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int CorpsesSpentThisGame { get; init; }
    [JsonIgnore]
    public int AvailableCorpses => Math.Max(0, Corpses.GetValueOrDefault() - CorpsesSpentThisGame);
    public ImmutableDictionary<GameZone, ImmutableArray<EntityId>> Zones { get; init; } =
        ImmutableDictionary<GameZone, ImmutableArray<EntityId>>.Empty;

    public ImmutableArray<EntityId> In(GameZone zone) =>
        Zones.TryGetValue(zone, out var entities) ? entities : ImmutableArray<EntityId>.Empty;
}

public enum MatchOutcome { InProgress, Draw, Won }

/// <summary>动作结算完成后用于继续对局的计数器和结果。</summary>
public sealed record GameContinuation(int NextEntity, long NextPlayOrder, MatchOutcome Outcome, PlayerId? Winner)
{
    public PendingPlayChoice? PendingPlayChoice { get; init; }
}

/// <summary>不可变对局状态。各区域按顺序存放实体表中的 ID。</summary>
public sealed class GameState
{
    private readonly ImmutableDictionary<EntityId, EntityLocation> _locations;

    private GameState(ImmutableDictionary<PlayerId, PlayerState> players,
        ImmutableDictionary<EntityId, GameEntity> entities,
        ImmutableDictionary<EntityId, EntityLocation> locations, PlayerId activePlayer,
        int turn, int nextEntity, long nextPlayOrder, MatchOutcome outcome, PlayerId? winner,
        ImmutableList<MinionDeath> minionDeaths, ulong randomState, PendingPlayChoice? pendingPlayChoice = null)
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
        PendingPlayChoice = pendingPlayChoice;
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
    public PendingPlayChoice? PendingPlayChoice { get; }

    internal static GameState Create(IEnumerable<PlayerState> players, IEnumerable<GameEntity> entities,
        PlayerId activePlayer, int turn, IEnumerable<MinionDeath>? minionDeaths, ulong randomState,
        GameContinuation? continuation = null)
    {
        if (players is null)
            throw new ArgumentNullException(nameof(players), "必须提供玩家列表。");
        if (entities is null)
            throw new ArgumentNullException(nameof(entities), "必须提供实体列表。");

        var playerIndex = ImmutableDictionary.CreateBuilder<PlayerId, PlayerState>();
        foreach (var player in players)
        {
            if (player is null)
                throw new ArgumentException("玩家列表中存在空条目。", nameof(players));
            if (!playerIndex.TryAdd(player.Id, NormalizeMana(player)))
                throw new ArgumentException($"玩家 ID {player.Id.Value} 重复。", nameof(players));
        }
        var byPlayer = playerIndex.ToImmutable();

        var entityIndex = ImmutableDictionary.CreateBuilder<EntityId, GameEntity>();
        foreach (var entity in entities)
        {
            if (entity is null)
                throw new ArgumentException("实体列表中存在空条目。", nameof(entities));
            if (!entityIndex.TryAdd(entity.Id, entity))
                throw new ArgumentException($"实体 ID {entity.Id} 重复。", nameof(entities));
        }
        var byEntity = entityIndex.ToImmutable();
        if (byPlayer.Count != 2 || !byPlayer.ContainsKey(activePlayer) || turn < 1)
            throw new ArgumentException("对局需要两名玩家、有效的当前回合玩家和大于零的回合数。");

        var locations = ImmutableDictionary.CreateBuilder<EntityId, EntityLocation>();
        var playOrders = new HashSet<long>();
        var survivors = new List<PlayerId>(2);
        foreach (var player in byPlayer.Values)
        {
            if (!byEntity.TryGetValue(player.Hero, out var hero) || hero.Character is not { } character)
                throw new ArgumentException($"玩家 {player.Id.Value} 缺少英雄或英雄属性。");
            if (player.In(GameZone.Play).Contains(player.Hero) && character.Health > 0)
                survivors.Add(player.Id);
            else if (continuation is null || !player.In(GameZone.Graveyard).Contains(player.Hero) || character.Health > 0)
                throw new ArgumentException($"玩家 {player.Id.Value} 的英雄生命值或所在区域无效。");
            if (player.EquippedWeapon is EntityId equipped && (!player.In(GameZone.Play).Contains(equipped) ||
                !byEntity.TryGetValue(equipped, out var weapon) || weapon.Weapon is not { Durability: > 0 }))
                throw new ArgumentException($"玩家 {player.Id.Value} 装备的武器无效。");
            if (player.In(GameZone.HeroPower).Length > 1 ||
                (player.In(GameZone.HeroPower).IsEmpty && player.HeroPowerUsesThisTurn != 0))
                throw new ArgumentException($"玩家 {player.Id.Value} 的英雄技能区域或使用次数无效。");

            foreach (var zone in player.Zones)
            {
                if (!Enum.IsDefined(zone.Key) || zone.Value.IsDefault)
                    throw new ArgumentException($"玩家 {player.Id.Value} 存在未初始化或类型未知的区域。");

                foreach (var id in zone.Value)
                {
                    if (!byEntity.TryGetValue(id, out var entity))
                        throw new ArgumentException($"区域引用了不存在的实体 {id}。");
                    ValidateEntity(entity, byPlayer);
                    if (entity.Controller != player.Id || !locations.TryAdd(id, new(player.Id, zone.Key)))
                        throw new ArgumentException($"实体 {id} 必须位于其控制者的一个且仅一个区域中。");

                    var hasActiveOrder = zone.Key == GameZone.Secret ||
                        (zone.Key == GameZone.Play && HasPlayState(entity));
                    if (hasActiveOrder &&
                        (entity.PlayOrder is not > 0 || !playOrders.Add(entity.PlayOrder.Value)))
                        throw new ArgumentException($"实体 {id} 的进场顺序必须为正数，且不能与其他实体重复。");
                }
            }
        }
        if (locations.Count != byEntity.Count)
            throw new ArgumentException("每个已登记的实体都必须位于一个区域。");

        var deaths = minionDeaths?.ToImmutableList() ?? ImmutableList<MinionDeath>.Empty;
        var deathTurn = 1;
        foreach (var death in deaths)
        {
            if (death is null || death.Turn < deathTurn || death.Turn > turn || string.IsNullOrWhiteSpace(death.CardId) ||
                !byEntity.ContainsKey(death.Entity) || !byPlayer.ContainsKey(death.Controller))
                throw new ArgumentException("随从死亡记录无效。", nameof(minionDeaths));
            deathTurn = death.Turn;
        }

        var minimumEntity = checked(byEntity.Keys.Max(id => id.Value) + 1);
        var minimumPlayOrder = checked(byEntity.Values.Max(e => e.PlayOrder ?? 0) + 1);
        var outcome = survivors.Count switch { 0 => MatchOutcome.Draw, 1 => MatchOutcome.Won, _ => MatchOutcome.InProgress };
        PlayerId? winner = survivors.Count == 1 ? survivors[0] : null;
        if (continuation is not null)
        {
            if (continuation.NextEntity < minimumEntity || continuation.NextPlayOrder < minimumPlayOrder)
                throw new ArgumentException("存档的实体 ID 或进场顺序计数会与已有实体冲突。", nameof(continuation));
            if (continuation.Outcome != outcome || continuation.Winner != winner)
                throw new ArgumentException("存档的对局结果与双方英雄的状态不一致。", nameof(continuation));
        }

        var state = new GameState(byPlayer, byEntity, locations.ToImmutable(), activePlayer, turn,
            continuation?.NextEntity ?? minimumEntity, continuation?.NextPlayOrder ?? minimumPlayOrder,
            outcome, winner, deaths, randomState);
        return continuation?.PendingPlayChoice is { } choice ? state.WithPendingPlayChoice(choice) : state;
    }

    internal GameState WithPendingPlayChoice(PendingPlayChoice? choice)
    {
        if (choice is not null &&
            (Outcome != MatchOutcome.InProgress || !_locations.TryGetValue(choice.Source, out var source) ||
                source != new EntityLocation(ActivePlayer, GameZone.Play) ||
                choice.Options.IsDefaultOrEmpty || choice.Options.Distinct().Count() != choice.Options.Length ||
                choice.Options.Any(id => !_locations.TryGetValue(id, out var option) || option != new EntityLocation(ActivePlayer, GameZone.SetAside)) ||
                choice.AfterPlayListeners.IsDefault || choice.AfterPlayListeners.Select(listener => listener?.Entity).Distinct().Count() != choice.AfterPlayListeners.Length ||
                choice.AfterPlayListeners.Any(listener => listener is null || !Entities.TryGetValue(listener.Entity, out var entity) ||
                    listener.CardId != entity.CardId || listener.Controller != entity.Controller || listener.PlayOrder != entity.PlayOrder ||
                    listener.Zone is not (GameZone.Play or GameZone.Secret) || LocationOf(listener.Entity).Zone != listener.Zone)))
            throw new ArgumentException("待决选择的来源、候选或后续触发记录与局面不一致。", nameof(choice));

        return new(Players, Entities, _locations, ActivePlayer, Turn, NextEntity, NextPlayOrder,
            Outcome, Winner, MinionDeaths, RandomState, choice);
    }

    public EntityLocation LocationOf(EntityId id) => _locations[id];

    public PlayerId OpponentOf(PlayerId player)
    {
        if (!Players.ContainsKey(player))
            throw new KeyNotFoundException($"对局中找不到玩家 {player.Value}。");
        return Players.Keys.Single(id => id != player);
    }

    // 所有规则共用这些结构操作。改变控制者时保留原有的拥有者。
    internal GameState Move(EntityId id, EntityLocation destination, int? index = null)
    {
        var origin = LocationOf(id);
        var entity = Entities[id];
        if (Players[origin.Player].Hero == id && destination.Player != origin.Player)
            throw new InvalidOperationException("英雄不能作为卡牌转移。");

        var sourcePlayer = Players[origin.Player];
        if (sourcePlayer.EquippedWeapon == id && origin != destination)
            sourcePlayer = sourcePlayer with { EquippedWeapon = null };
        var players = Players.SetItem(sourcePlayer.Id, SetZone(sourcePlayer, origin.Zone, sourcePlayer.In(origin.Zone).Remove(id)));
        var to = players[destination.Player];
        var zone = to.In(destination.Zone);
        to = SetZone(to, destination.Zone, zone.Insert(index ?? zone.Length, id));
        var entersActiveZone = (destination.Zone is GameZone.Play or GameZone.Secret) && destination.Zone != origin.Zone;
        entity = entity with
        {
            Controller = destination.Player,
            PlayOrder = entersActiveZone ? NextPlayOrder : entity.PlayOrder,
            AuraGrants = destination.Zone == GameZone.Play ? entity.AuraGrants : [],
            Enchantments = destination.Zone == GameZone.Play ? entity.Enchantments : [],
            Stealthed = destination.Zone == GameZone.Play ? entity.Stealthed : null,
            CostModifiers = destination.Zone == GameZone.Hand ? entity.CostModifiers : [],
            CostTypeModifiers = destination.Zone == GameZone.Hand ? entity.CostTypeModifiers : null
        };
        return new GameState(players.SetItem(to.Id, to), Entities.SetItem(id, entity), _locations.SetItem(id, destination),
            ActivePlayer, Turn, NextEntity, entersActiveZone ? checked(NextPlayOrder + 1) : NextPlayOrder,
            Outcome, Winner, MinionDeaths, RandomState, PendingPlayChoice);
    }

    internal GameState UpdateCharacter(EntityId id, CharacterState character) =>
        UpdateEntity(Entities[id] with { Character = character });

    internal GameState UpdateEntity(GameEntity entity)
    {
        if (!Entities.TryGetValue(entity.Id, out var current) || entity.Owner != current.Owner ||
            entity.Controller != current.Controller || entity.PlayOrder != current.PlayOrder)
            throw new ArgumentException("实体标识和位置只能通过对局结构操作更改。", nameof(entity));
        ValidateEntity(entity, Players);
        var players = Players;
        var owner = players[entity.Controller];
        if (owner.Hero == entity.Id && current.Character?.Health != entity.Character?.Health)
            players = players.SetItem(owner.Id, owner with { HeroHealthChangedThisTurn = true });
        return new GameState(players, Entities.SetItem(entity.Id, entity), _locations,
            ActivePlayer, Turn, NextEntity, NextPlayOrder, Outcome, Winner, MinionDeaths, RandomState, PendingPlayChoice);
    }

    internal GameState AddToPlay(GameEntity entity, int index) => AddEntity(entity, GameZone.Play, index);

    internal GameState AddToHand(GameEntity entity) => AddEntity(entity, GameZone.Hand);
    internal GameState AddChoiceOption(GameEntity entity) => AddEntity(entity, GameZone.SetAside);

    internal GameState AddToDeck(GameEntity entity, int index) => AddEntity(entity, GameZone.Deck, index);

    internal GameState ReorderDeck(PlayerId playerId, ImmutableArray<EntityId> order)
    {
        var player = Players[playerId];
        var deck = player.In(GameZone.Deck);
        if (order.IsDefault || order.Length != deck.Length || !deck.ToHashSet().SetEquals(order))
            throw new ArgumentException("调整牌序时必须保留牌库中的所有实体。", nameof(order));
        return UpdatePlayer(SetZone(player, GameZone.Deck, order));
    }

    private GameState AddEntity(GameEntity entity, GameZone zone, int? index = null)
    {
        ValidateEntity(entity, Players);
        var entersPlay = zone == GameZone.Play;
        if (entity.Id.Value != NextEntity || entersPlay != HasPlayState(entity))
            throw new ArgumentException("新实体必须使用下一个可用 ID，且其组件必须符合所在区域的要求。");

        var player = Players[entity.Controller];
        var contents = player.In(zone);
        player = SetZone(player, zone, contents.Insert(index ?? contents.Length, entity.Id));
        return new GameState(Players.SetItem(player.Id, player),
            Entities.Add(entity.Id, entity with { PlayOrder = entersPlay ? NextPlayOrder : null }),
            _locations.Add(entity.Id, new(player.Id, zone)), ActivePlayer, Turn,
            checked(NextEntity + 1), entersPlay ? checked(NextPlayOrder + 1) : NextPlayOrder, Outcome, Winner, MinionDeaths, RandomState, PendingPlayChoice);
    }

    internal GameState UpdateResources(PlayerState player)
    {
        var current = Players[player.Id];
        if (player.Zones != current.Zones || player.Hero != current.Hero || player.EquippedWeapon != current.EquippedWeapon || player.ManaSpent is null)
            throw new ArgumentException("更新资源时需要保留区域、英雄和武器，并明确提供法力计数。", nameof(player));
        ValidateResources(player);
        return UpdatePlayer(player);
    }

    internal GameState EquipWeapon(PlayerId player, EntityId id)
    {
        if (LocationOf(id) != new EntityLocation(player, GameZone.Play) || Entities[id].Weapon is not { Durability: > 0 })
            throw new ArgumentException("武器必须位于场上，且耐久大于零，才能装备。", nameof(id));
        return UpdatePlayer(Players[player] with { EquippedWeapon = id });
    }

    private GameState UpdatePlayer(PlayerState player) =>
        new(Players.SetItem(player.Id, player), Entities, _locations,
            ActivePlayer, Turn, NextEntity, NextPlayOrder, Outcome, Winner, MinionDeaths, RandomState, PendingPlayChoice);

    internal GameState RecordMinionDeath(GameEntity minion, bool leavesCorpse)
    {
        var players = Players;
        if (leavesCorpse)
        {
            var player = players[minion.Controller];
            players = players.SetItem(player.Id, player with { Corpses = checked(player.Corpses.GetValueOrDefault() + 1) });
        }
        return new(players, Entities, _locations, ActivePlayer, Turn, NextEntity, NextPlayOrder, Outcome, Winner,
            MinionDeaths.Add(new(minion.Id, minion.CardId, minion.Controller, Turn)), RandomState, PendingPlayChoice);
    }

    internal GameState SetRandomState(ulong randomState) => new(Players, Entities, _locations, ActivePlayer, Turn,
        NextEntity, NextPlayOrder, Outcome, Winner, MinionDeaths, randomState, PendingPlayChoice);

    internal GameState PassTurn() => new(Players, Entities, _locations, OpponentOf(ActivePlayer),
        checked(Turn + 1), NextEntity, NextPlayOrder, Outcome, Winner, MinionDeaths, RandomState);

    internal GameState Finish(PlayerId? winner)
    {
        if (winner is PlayerId id && !Players.ContainsKey(id))
            throw new ArgumentException("胜者必须是本局的玩家。", nameof(winner));
        return new GameState(Players, Entities, _locations, ActivePlayer, Turn, NextEntity, NextPlayOrder,
            winner is null ? MatchOutcome.Draw : MatchOutcome.Won, winner, MinionDeaths, RandomState);
    }

    private static PlayerState SetZone(PlayerState player, GameZone zone, ImmutableArray<EntityId> contents)
    {
        if (!Enum.IsDefined(zone))
            throw new ArgumentOutOfRangeException(nameof(zone), "对局区域无效。");
        return player with { Zones = player.Zones.SetItem(zone, contents) };
    }

    private static void ValidateResources(PlayerState player)
    {
        if (player.Zones is null || player.Id.Value < 1 || player.Mana < 0 || player.ManaCrystals < 0 ||
            player.MaximumManaCrystals < 1 || player.ManaCrystals > player.MaximumManaCrystals ||
            player.Fatigue < 0 || player.HeroPowerUsesThisTurn is < 0 or > 1 || player.ManaSpent is < 0 ||
            player.TemporaryMana < 0 || player.OverloadLocked < 0 || player.OverloadOwed < 0 ||
            player.Corpses is < 0 || player.CorpsesSpentThisGame < 0)
            throw new ArgumentException("玩家 ID 或资源数值无效。");
    }

    private static PlayerState NormalizeMana(PlayerState player)
    {
        ValidateResources(player);
        if (player.ManaSpent is not null)
            return player with { Mana = 0 };

        // 旧回放只提供界面显示的法力值。换算时保留该值和待结算的过载。
        var unspent = (long)player.ManaCrystals + player.TemporaryMana - player.OverloadLocked;
        var spent = Math.Max(0, unspent - player.Mana);
        var temporary = player.Mana == 0 ? 0 : Math.Max(0, player.Mana - unspent);
        return player with { Mana = 0, ManaSpent = checked((int)spent), TemporaryMana = checked(player.TemporaryMana + (int)temporary) };
    }

    private static void ValidateEntity(GameEntity entity, IReadOnlyDictionary<PlayerId, PlayerState> players)
    {
        if (entity.Id.Value < 0 || string.IsNullOrWhiteSpace(entity.CardId) ||
            !players.ContainsKey(entity.Owner) || !players.ContainsKey(entity.Controller) || entity.PlayOrder is < 1 ||
            entity.Enchantments.IsDefault || entity.AuraGrants.IsDefault || entity.CostModifiers.IsDefault)
            throw new ArgumentException($"实体 {entity.Id} 的标识、拥有者、控制者或进场顺序无效。");
        if (entity.Character is { } character)
            ValidateCharacter(character);
        if (entity.Weapon is { } weapon && (entity.Character is not null || weapon.Attack < 0 || weapon.Durability < 0 ||
            weapon.MaximumDurability < 1 || weapon.Durability > weapon.MaximumDurability || weapon.AttacksThisTurn < 0))
            throw new ArgumentException($"实体 {entity.Id} 的武器属性无效。");
        if (entity.Location is { } location && (entity.Character is not null || entity.Weapon is not null ||
            location.Health < 0 || location.TurnsUntilReady is < 0 or > LocationState.StartsUntilReadyAfterUse ||
            (location.Health == 0 && location.TurnsUntilReady != 0)))
            throw new ArgumentException($"实体 {entity.Id} 的地标属性无效。");
    }

    private static bool HasPlayState(GameEntity entity) =>
        entity.Character is not null || entity.Weapon is not null || entity.Location is not null;

    private static void ValidateCharacter(CharacterState character)
    {
        if (character.Attack < 0 || character.MaximumHealth < 1 || character.Damage < 0 ||
            character.Armor < 0 || character.AttacksThisTurn < 0 ||
            character.BaseAttack is < 0 || character.BaseMaximumHealth is < 1)
            throw new ArgumentException("角色属性无效。");
    }
}
