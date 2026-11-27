using System.Globalization;
using System.Text.Json;
using Silverfish.Cli.Web;
using Silverfish.Contracts;
using Silverfish.Domain.Client;

namespace Silverfish.Cli;

internal static class StandaloneSnapshotAdapter
{
    private static readonly JsonSerializerOptions InputOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // TAG_STEP from the client. Intermediate action and animation steps remain Unknown.
    private enum ClientStep { Mulligan = 4, MainAction = 10, FinalWrapUp = 14, FinalGameOver = 15 }

    internal static async Task<GameSnapshot> ReadAsync(Stream input, string matchId)
    {
        using var document = await JsonDocument.ParseAsync(input);
        return Read(document.RootElement, matchId).Snapshot;
    }

    internal static (GameSnapshot Snapshot, ConsoleEntity[] AllEntities, ConsolePendingChoice? PendingChoice,
        bool CanAct, string? BlockReason, string? ResponseMode) Read(JsonElement payload, string matchId)
    {
        if (string.IsNullOrWhiteSpace(matchId))
            matchId = "imported-match";
        if (payload.ValueKind == JsonValueKind.Object &&
            payload.TryGetProperty("payloadJson", out var payloadJsonProp) &&
            payloadJsonProp.ValueKind == JsonValueKind.String)
        {
            using var nestedDoc = JsonDocument.Parse(payloadJsonProp.GetString()!);
            return Read(nestedDoc.RootElement, matchId);
        }
        var frame = payload.Deserialize<StateFrame>(InputOptions)
            ?? throw new InvalidDataException("必须提供 state.snapshot 的 payload。");
        if (frame.ProtocolVersion != 1 || frame.Revision < 0)
            throw new InvalidDataException("state.snapshot 的 payload 版本或序号无效。");
        if (frame.Match is null)
            throw new InvalidDataException("快照缺少 match 对象。");
        var canAct = frame.CanAct ?? true;
        if (!frame.Match.GameActive)
        {
            var idle = new GameSnapshot
            {
                MatchId = matchId,
                Sequence = frame.Revision,
                Phase = GamePhase.Unknown,
                IsOurTurn = false,
                AvailableMana = 0,
                MaximumMana = 0,
                OurHero = new HeroSnapshot { EntityId = 1, Health = 30, Armor = 0, Attack = 0, ClassNameZh = "未开局" },
                EnemyHero = new HeroSnapshot { EntityId = 2, Health = 30, Armor = 0, Attack = 0, ClassNameZh = "等待对局" },
                OurBoard = [],
                EnemyBoard = [],
                OurHand = []
            };
            return (idle, [], null, canAct, frame.BlockReason, frame.ResponseMode);
        }
        if (frame.Match.Players is not { Length: 2 } players || players.Any(player => player is null) ||
            players[0].PlayerId == players[1].PlayerId || players[0].EntityId == players[1].EntityId ||
            players[0].HeroId == players[1].HeroId)
            throw new InvalidDataException("快照必须提供两个不同的玩家及英雄。");
        var friendly = FindPlayer(players, "Friendly", frame.Match.FriendlyPlayerId, frame.Match.FriendlyEntityId);
        var opponent = FindPlayer(players, "Opponent", frame.Match.OpponentPlayerId, frame.Match.OpponentEntityId);
        var entities = IndexEntities(frame.Entities);
        var opponentPrivateIds = (opponent.Zones ?? [])
            .Where(zone => zone?.Name is "Hand" or "Deck" or "Secrets" or "SetAside")
            .SelectMany(zone => zone!.EntityIds ?? []).ToHashSet();
        var choiceIds = frame.PendingChoice?.Cards?.Select(c => c.EntityId).ToHashSet() ?? [];
        var hidden = entities.Values.Where(entity => !choiceIds.Contains(entity.Id) &&
                (opponentPrivateIds.Contains(entity.Id) || entity.IsPrivateTo(friendly.PlayerId)))
            .Select(entity => entity.Id).ToHashSet();

        var friendlyPlayer = FindEntity(entities, friendly.EntityId, TAG_CARDTYPE.PLAYER);
        var opponentPlayer = FindEntity(entities, opponent.EntityId, TAG_CARDTYPE.PLAYER);
        var friendlyTurn = friendlyPlayer.Has(GAME_TAG.CURRENT_PLAYER);
        var opponentTurn = opponentPlayer.Has(GAME_TAG.CURRENT_PLAYER);

        var crystals = friendlyPlayer.NonNegative(GAME_TAG.RESOURCES);
        var temporaryMana = friendlyPlayer.NonNegative(GAME_TAG.TEMP_RESOURCES);
        var spent = friendlyPlayer.NonNegative(GAME_TAG.RESOURCES_USED);
        var availableMana = Math.Max(0, checked(crystals + temporaryMana - spent));
        var snapshot = new GameSnapshot
        {
            MatchId = matchId,
            Sequence = frame.Revision,
            Phase = GetPhase(entities.Values),
            IsOurTurn = friendlyTurn && !opponentTurn,
            AvailableMana = availableMana,
            MaximumMana = availableMana <= crystals ? crystals : null,
            OurHero = ReadHero(entities, friendly),
            EnemyHero = ReadHero(entities, opponent),
            OurBoard = ReadBoard(entities, hidden, friendly),
            EnemyBoard = ReadBoard(entities, hidden, opponent),
            OurHand = VisibleEntities(entities, hidden, friendly, "Hand", TAG_ZONE.HAND)
                .Where(card => card.ReadTag(GAME_TAG.COST) is >= 0)
                .Select(card => new HandCardSnapshot
                {
                    EntityId = card.Id, CardId = card.CardId!, CardNameZh = card.Name ?? string.Empty, Cost = card.Value(GAME_TAG.COST)
                }).ToArray(),
            EnemyHandCount = ZoneCount(opponent, "Hand"),
            EnemyDeckCount = ZoneCount(opponent, "Deck"),
            OurSecretCount = ZoneCount(friendly, "Secrets"),
            EnemySecretCount = ZoneCount(opponent, "Secrets"),
            LegalActions = ReadLegalActions(frame.LegalActions)
        };
        var pendingChoice = frame.PendingChoice is { Cards.Length: > 0 } pc
            ? new ConsolePendingChoice(pc.ChoiceId, pc.ChoiceType ?? "GENERAL", pc.SourceEntityId, pc.CountMin, pc.CountMax,
                pc.IsSingleChoice, pc.HasConfirmButton,
                [.. pc.Cards.Select(c => new ConsoleChoiceCard(c.EntityId, c.CardId ?? string.Empty,
                    string.IsNullOrWhiteSpace(c.Name) ? (c.CardId ?? $"#{c.EntityId}") : c.Name!, c.CardType, c.Cost, c.Attack, c.Health, c.Unchoosable, c.Chosen))])
            : null;
        return (snapshot, entities.Values.Select(entity => Describe(entity, hidden.Contains(entity.Id))).ToArray(), pendingChoice,
            canAct, frame.BlockReason, frame.ResponseMode);
    }

    private static CandidateAction[] ReadLegalActions(StateLegalAction[]? actions)
    {
        if (actions is not { Length: > 0 })
            return [];
        var result = new List<CandidateAction>(actions.Length);
        foreach (var act in actions)
        {
            if (act is null || string.IsNullOrWhiteSpace(act.Id)) continue;
            var kind = act.Kind switch
            {
                "endTurn" or "end_turn" => GameActionKind.EndTurn,
                "attack" => GameActionKind.Attack,
                "heroPower" or "hero_power" or "useHeroPower" => GameActionKind.HeroPower,
                "activateLocation" or "activate_location" => GameActionKind.ActivateLocation,
                _ => GameActionKind.PlayCard
            };
            result.Add(new CandidateAction
            {
                Id = act.Id,
                Kind = kind,
                SourceEntityId = act.SourceEntityId,
                TargetEntityId = act.TargetEntityId,
                Position = act.Position,
                Description = act.Description ?? string.Empty
            });
        }
        return [.. result];
    }

    private static ConsoleEntity Describe(StateEntity entity, bool hidden) => new(entity.Id, hidden ? null : entity.CardId,
        entity.ControllerId, entity.ZoneKind?.ToString() ?? entity.Zone, entity.ZonePosition, hidden ? null : entity.CardType,
        hidden, hidden ? [] : DescribeTags(entity.Tags), hidden ? [] : DescribeTags(entity.CachedTagsForDormant));

    private static ConsoleEntityTag[] DescribeTags(EntityTag?[]? tags) => tags is null ? [] :
        [.. tags.OfType<EntityTag>().Select(tag => new ConsoleEntityTag(tag.Id,
            Enum.GetName((GAME_TAG)tag.Id) ?? "TAG_" + tag.Id.ToString(CultureInfo.InvariantCulture), tag.Value))];

    private static StatePlayer FindPlayer(StatePlayer[] players, string side, int playerId, int entityId) =>
        players.FirstOrDefault(player => player.Side == side && playerId > 0 && player.PlayerId == playerId &&
            entityId > 0 && player.EntityId == entityId && player.HeroId > 0)
        ?? throw new InvalidDataException($"快照的 {side} 玩家标识不一致。");

    private static Dictionary<int, StateEntity> IndexEntities(StateEntity[]? entities)
    {
        if (entities is null)
            throw new InvalidDataException("快照缺少实体表。");
        var result = new Dictionary<int, StateEntity>();
        foreach (var entity in entities)
        {
            if (entity is null || entity.Id <= 0 || !result.TryAdd(entity.Id, entity))
                throw new InvalidDataException("快照包含无效或重复的实体 ID。");
        }
        return result;
    }

    private static GamePhase GetPhase(IEnumerable<StateEntity> entities)
    {
        var game = entities.FirstOrDefault(entity => entity.CardType == nameof(TAG_CARDTYPE.GAME));
        return (ClientStep)(game?.Value(GAME_TAG.STEP) ?? 0) switch
        {
            ClientStep.Mulligan => GamePhase.Mulligan,
            ClientStep.MainAction => GamePhase.Main,
            ClientStep.FinalWrapUp or ClientStep.FinalGameOver => GamePhase.GameOver,
            _ => GamePhase.Unknown
        };
    }

    private static HeroSnapshot ReadHero(Dictionary<int, StateEntity> entities, StatePlayer player)
    {
        var hero = FindEntity(entities, player.HeroId, TAG_CARDTYPE.HERO);
        if (hero.ControllerId != player.PlayerId)
            throw new InvalidDataException($"英雄实体 {hero.Id} 的操控者与快照不一致。");
        return new HeroSnapshot
        {
            EntityId = hero.Id,
            Health = RemainingHealth(hero),
            Armor = hero.NonNegative(GAME_TAG.ARMOR),
            Attack = hero.NonNegative(GAME_TAG.ATK),
            ClassNameZh = (TAG_CLASS)hero.Value(GAME_TAG.CLASS) switch
            {
                TAG_CLASS.DEATHKNIGHT => "死亡骑士",
                TAG_CLASS.DRUID => "德鲁伊",
                TAG_CLASS.HUNTER => "猎人",
                TAG_CLASS.MAGE => "法师",
                TAG_CLASS.PALADIN => "圣骑士",
                TAG_CLASS.PRIEST => "牧师",
                TAG_CLASS.ROGUE => "潜行者",
                TAG_CLASS.SHAMAN => "萨满祭司",
                TAG_CLASS.WARLOCK => "术士",
                TAG_CLASS.WARRIOR => "战士",
                TAG_CLASS.DEMONHUNTER => "恶魔猎手",
                _ => string.Empty
            }
        };
    }

    private static MinionSnapshot[] ReadBoard(Dictionary<int, StateEntity> entities, HashSet<int> hidden, StatePlayer player) =>
        VisibleEntities(entities, hidden, player, "Battlefield", TAG_ZONE.PLAY)
            .Where(entity => entity.CardType == nameof(TAG_CARDTYPE.MINION) && entity.ReadTag(GAME_TAG.HEALTH) is >= 0)
            .Select(entity => new MinionSnapshot
            {
                EntityId = entity.Id, CardId = entity.CardId!, CardNameZh = entity.Name ?? string.Empty,
                Attack = Math.Max(0, entity.Value(GAME_TAG.ATK)),
                Health = Math.Max(0, entity.Value(GAME_TAG.HEALTH) - Math.Max(0, entity.Value(GAME_TAG.DAMAGE))),
                HasTaunt = entity.Has(GAME_TAG.TAUNT),
                HasDivineShield = entity.Has(GAME_TAG.DIVINE_SHIELD)
            }).ToArray();

    private static int RemainingHealth(StateEntity entity) =>
        Math.Max(0, entity.RequiredNonNegative(GAME_TAG.HEALTH) - entity.NonNegative(GAME_TAG.DAMAGE));

    private static StateEntity FindEntity(Dictionary<int, StateEntity> entities, int id, TAG_CARDTYPE type) =>
        entities.TryGetValue(id, out var entity) && entity.CardType == type.ToString()
            ? entity : throw new InvalidDataException($"快照缺少类型为 {type} 的实体 {id}。");

    private static PlayerZone? FindZone(StatePlayer player, string name) =>
        player.Zones?.FirstOrDefault(zone => zone?.Name == name);

    private static int? ZoneCount(StatePlayer player, string name) =>
        FindZone(player, name)?.Count is >= 0 and var count ? count : null;

    private static IEnumerable<StateEntity> VisibleEntities(
        Dictionary<int, StateEntity> entities, HashSet<int> hidden, StatePlayer player, string zoneName, TAG_ZONE zoneType)
    {
        var candidates = entities.Values.Where(entity => entity.ControllerId == player.PlayerId && entity.ZoneKind == zoneType &&
            !hidden.Contains(entity.Id) && !string.IsNullOrWhiteSpace(entity.CardId)).ToDictionary(entity => entity.Id);
        foreach (var id in FindZone(player, zoneName)?.EntityIds ?? [])
        {
            if (candidates.Remove(id, out var entity))
                yield return entity;
        }
        foreach (var entity in candidates.Values.OrderBy(entity => entity.ZonePosition ?? int.MaxValue).ThenBy(entity => entity.Id))
            yield return entity;
    }

    private sealed class StateFrame
    {
        public required int ProtocolVersion { get; init; }
        public required long Revision { get; init; }
        public required MatchState Match { get; init; }
        public required StateEntity[] Entities { get; init; }
        public StatePendingChoice? PendingChoice { get; init; }
        public StateLegalAction[]? LegalActions { get; init; }
        public bool? CanAct { get; init; }
        public string? BlockReason { get; init; }
        public string? ResponseMode { get; init; }
    }

    private sealed class StatePendingChoice
    {
        public int ChoiceId { get; init; }
        public string? ChoiceType { get; init; }
        public int SourceEntityId { get; init; }
        public int CountMin { get; init; }
        public int CountMax { get; init; }
        public bool IsSingleChoice { get; init; }
        public bool HasConfirmButton { get; init; }
        public StateChoiceCard[]? Cards { get; init; }
    }

    private sealed class StateChoiceCard
    {
        public int EntityId { get; init; }
        public string? CardId { get; init; }
        public string? Name { get; init; }
        public string? CardType { get; init; }
        public int Cost { get; init; }
        public int? Attack { get; init; }
        public int? Health { get; init; }
        public bool Unchoosable { get; init; }
        public bool Chosen { get; init; }
    }

    private sealed class StateLegalAction
    {
        public string? Id { get; init; }
        public string? Kind { get; init; }
        public int? SourceEntityId { get; init; }
        public int? TargetEntityId { get; init; }
        public int? Position { get; init; }
        public string? Description { get; init; }
    }

    private sealed class MatchState
    {
        public required bool GameActive { get; init; }
        public required int FriendlyPlayerId { get; init; }
        public required int OpponentPlayerId { get; init; }
        public required int FriendlyEntityId { get; init; }
        public required int OpponentEntityId { get; init; }
        public required StatePlayer[] Players { get; init; }
    }

    private sealed class StatePlayer
    {
        public required string Side { get; init; }
        public required int EntityId { get; init; }
        public required int PlayerId { get; init; }
        public required int HeroId { get; init; }
        public PlayerZone?[]? Zones { get; init; }
    }

    private sealed class PlayerZone
    {
        public string? Name { get; init; }
        public int? Count { get; init; }
        public int[]? EntityIds { get; init; }
    }

    private sealed class StateEntity
    {
        public required int Id { get; init; }
        public string? CardId { get; init; }
        public string? Name { get; init; }
        public int ControllerId { get; init; }
        public string? Zone { get; init; }
        public int? ZonePosition { get; init; }
        public string? CardType { get; init; }
        public EntityTag?[]? Tags { get; init; }
        public EntityTag?[]? CachedTagsForDormant { get; init; }

        internal TAG_ZONE? ZoneKind => Enum.TryParse<TAG_ZONE>(Zone, ignoreCase: true, out var zone) && Enum.IsDefined(zone)
            ? zone : ReadTag(GAME_TAG.ZONE) is int tag && Enum.IsDefined((TAG_ZONE)tag) ? (TAG_ZONE)tag : null;

        internal bool IsPrivateTo(int friendlyId) =>
            (ControllerId != friendlyId || Tags?.Any(tag => tag?.Id == (int)GAME_TAG.CONTROLLER && tag.Value != friendlyId) == true) &&
            (IsPrivateZone(ZoneKind) || Tags?.Any(tag => tag?.Id == (int)GAME_TAG.ZONE && IsPrivateZone((TAG_ZONE)tag.Value)) == true);

        private static bool IsPrivateZone(TAG_ZONE? zone) => zone is TAG_ZONE.HAND or TAG_ZONE.DECK or TAG_ZONE.SECRET or TAG_ZONE.SETASIDE;

        internal bool Has(GAME_TAG tag) => Value(tag) > 0;
        internal int Value(GAME_TAG tag) => ReadTag(tag) ?? 0;
        internal int NonNegative(GAME_TAG tag) => CheckNonNegative(tag, Value(tag));
        internal int RequiredNonNegative(GAME_TAG tag) => CheckNonNegative(tag, ReadTag(tag)
            ?? throw new InvalidDataException($"实体 {Id} 缺少标签 {tag}。"));

        internal int? ReadTag(GAME_TAG tag) => Tags?.FirstOrDefault(entry => entry?.Id == (int)tag)?.Value;

        private int CheckNonNegative(GAME_TAG tag, int value) => value >= 0
            ? value : throw new InvalidDataException($"实体 {Id} 的标签 {tag} 不能为负数。");
    }

    private sealed class EntityTag
    {
        public required int Id { get; init; }
        public required int Value { get; init; }
    }
}
