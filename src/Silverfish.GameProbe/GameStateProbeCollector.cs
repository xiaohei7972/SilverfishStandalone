using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Silverfish.GameProbe;

public sealed class ProbeTagSnapshot
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

public sealed class ProbeEntitySnapshot
{
    public int Id { get; set; }
    public string CardId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ControllerId { get; set; }
    public string Zone { get; set; } = string.Empty;
    public int ZonePosition { get; set; }
    public string CardType { get; set; } = string.Empty;
    public List<ProbeTagSnapshot> Tags { get; } = new List<ProbeTagSnapshot>();
    public List<ProbeTagSnapshot> CachedTagsForDormant { get; } = new List<ProbeTagSnapshot>();
}

public sealed class ProbeChoiceCardSnapshot
{
    public int EntityId { get; set; }
    public string CardId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public int Cost { get; set; }
    public int? Attack { get; set; }
    public int? Health { get; set; }
    public bool Unchoosable { get; set; }
    public bool Chosen { get; set; }
}

public sealed class ProbePendingChoiceSnapshot
{
    public int ChoiceId { get; set; }
    public string ChoiceType { get; set; } = string.Empty;
    public int SourceEntityId { get; set; }
    public int CountMin { get; set; }
    public int CountMax { get; set; }
    public bool IsSingleChoice { get; set; }
    public bool HasConfirmButton { get; set; }
    public List<ProbeChoiceCardSnapshot> Cards { get; } = new List<ProbeChoiceCardSnapshot>();
}

public sealed class ProbeObservedActionSnapshot
{
    public string Id { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public int? SourceEntityId { get; set; }
    public int? TargetEntityId { get; set; }
    public int? Position { get; set; }
    public string Description { get; set; } = string.Empty;
}

public sealed class ProbeZoneSnapshot
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public int HiddenCount { get; set; }
    public List<int> EntityIds { get; } = new List<int>();
}

public sealed class ProbePlayerSnapshot
{
    public string Side { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int PlayerId { get; set; }
    public int HeroId { get; set; }
    public int HeroPowerId { get; set; }
    public int WeaponId { get; set; }
    public List<ProbeZoneSnapshot> Zones { get; } = new List<ProbeZoneSnapshot>();
}

public sealed class ProbeMatchSnapshot
{
    public bool GameActive { get; set; }
    public int FriendlyPlayerId { get; set; }
    public int OpponentPlayerId { get; set; }
    public int FriendlyEntityId { get; set; }
    public int OpponentEntityId { get; set; }
    public List<ProbePlayerSnapshot> Players { get; } = new List<ProbePlayerSnapshot>();
}

public sealed class ProbeFrameSnapshot
{
    public int ProtocolVersion { get; set; } = 1;
    public long Revision { get; set; }
    public string CapturedAtUtc { get; set; } = string.Empty;
    public bool CanAct { get; set; }
    public string BlockReason { get; set; } = string.Empty;
    public string ResponseMode { get; set; } = string.Empty;
    public ProbeMatchSnapshot Match { get; set; } = new ProbeMatchSnapshot();
    public List<ProbeEntitySnapshot> Entities { get; } = new List<ProbeEntitySnapshot>();
    public ProbePendingChoiceSnapshot? PendingChoice { get; set; }
    public List<ProbeObservedActionSnapshot> LegalActions { get; } = new List<ProbeObservedActionSnapshot>();
}

public static class GameStateProbeCollector
{
    private const BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static ProbeFrameSnapshot? CaptureFromCurrentDomain(long revision, out int fingerprint)
    {
        fingerprint = 0;
        var gameStateType = FindGameStateType();
        if (gameStateType == null)
            return null;

        var getMethod = gameStateType.GetMethod("Get", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        var gameState = getMethod?.Invoke(null, null);
        return Capture(gameState, revision, out fingerprint);
    }

    public static ProbeFrameSnapshot CreateIdleFrame(long revision) => new ProbeFrameSnapshot
    {
        ProtocolVersion = 1,
        Revision = revision,
        CapturedAtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
        Match = new ProbeMatchSnapshot { GameActive = false }
    };

    public static ProbeFrameSnapshot? Capture(object? gameState, long revision, out int fingerprint)
    {
        fingerprint = 0;
        if (gameState == null)
            return null;

        var friendly = Invoke(gameState, "GetFriendlySidePlayer");
        var opponent = Invoke(gameState, "GetOpposingSidePlayer");
        var entityMapObj = Invoke(gameState, "GetEntityMap") ?? GetFieldOrProperty(gameState, "m_entityMap") ?? GetFieldOrProperty(gameState, "EntityMap");
        if (friendly == null || opponent == null || entityMapObj == null)
            return null;

        var friendlyPlayerId = ToInt(Invoke(friendly, "GetPlayerId") ?? GetFieldOrProperty(friendly, "PlayerId"));
        var friendlyEntityId = ToInt(Invoke(friendly, "GetEntityId") ?? GetFieldOrProperty(friendly, "EntityId"));
        var opponentPlayerId = ToInt(Invoke(opponent, "GetPlayerId") ?? GetFieldOrProperty(opponent, "PlayerId"));
        var opponentEntityId = ToInt(Invoke(opponent, "GetEntityId") ?? GetFieldOrProperty(opponent, "EntityId"));
        if (friendlyPlayerId <= 0 || opponentPlayerId <= 0 || friendlyEntityId <= 0 || opponentEntityId <= 0)
            return null;

        var frame = new ProbeFrameSnapshot
        {
            ProtocolVersion = 1,
            Revision = revision,
            CapturedAtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
            Match = new ProbeMatchSnapshot
            {
                GameActive = true,
                FriendlyPlayerId = friendlyPlayerId,
                OpponentPlayerId = opponentPlayerId,
                FriendlyEntityId = friendlyEntityId,
                OpponentEntityId = opponentEntityId
            }
        };

        var gameTagEnum = FindType("GAME_TAG");
        var choiceEntityIds = CollectFriendlyChoiceEntityIds(gameState);
        var entitiesById = new Dictionary<int, ProbeEntitySnapshot>();

        foreach (var rawEntity in EnumerateEntityMap(entityMapObj))
        {
            if (rawEntity == null)
                continue;
            var snapshot = BuildEntity(rawEntity, friendlyPlayerId, gameTagEnum, choiceEntityIds);
            if (snapshot != null && snapshot.Id > 0)
                entitiesById[snapshot.Id] = snapshot;
        }

        EnsurePlayerEntity(friendly, friendlyPlayerId, gameTagEnum, choiceEntityIds, entitiesById);
        EnsurePlayerEntity(opponent, friendlyPlayerId, gameTagEnum, choiceEntityIds, entitiesById);

        frame.Match.Players.Add(BuildPlayer(opponent, "Opponent", false, entitiesById));
        frame.Match.Players.Add(BuildPlayer(friendly, "Friendly", true, entitiesById));

        foreach (var entity in entitiesById.Values.OrderBy(e => e.Id))
            frame.Entities.Add(entity);

        frame.PendingChoice = BuildPendingChoice(gameState, entitiesById);
        BuildObservedActions(gameState, friendlyPlayerId, entitiesById, frame.LegalActions);

        var readiness = EvaluateActionReadiness(gameState, friendlyPlayerId, frame.PendingChoice != null, frame.LegalActions.Count > 0);
        frame.CanAct = readiness.CanAct;
        frame.BlockReason = readiness.BlockReason;
        frame.ResponseMode = readiness.ResponseMode;

        fingerprint = ComputeFingerprint(frame);
        return frame;
    }

    public static (bool CanAct, string BlockReason, string ResponseMode) EvaluateActionReadiness(
        object gameState, int friendlyPlayerId, bool hasPendingChoice, bool hasLegalActions)
    {
        var responseMode = Convert.ToString(Invoke(gameState, "GetResponseMode") ?? GetFieldOrProperty(gameState, "m_responseMode"), CultureInfo.InvariantCulture) ?? "UNKNOWN";
        try
        {
            if (VirtualInputExecutor.IsExecuting)
                return (false, "虚拟输入执行中", responseMode);
            if (ToBool(Invoke(gameState, "IsBusy")))
                return (false, "游戏正在结算(IsBusy)", responseMode);
            if (ToBool(Invoke(gameState, "IsBlockingPowerProcessor")))
                return (false, "法术/动画队列阻塞中", responseMode);
            if (ToBool(Invoke(gameState, "IsTurnStartManagerBlockingInput")))
                return (false, "回合开始动画中", responseMode);
            if (ToBool(Invoke(gameState, "IsResponsePacketBlocked")))
                return (false, $"响应被阻断({responseMode})", responseMode);

            if (hasPendingChoice)
            {
                var mgrType = FindType("ChoiceCardMgr");
                var mgr = mgrType?.GetMethod("Get", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(null, null);
                if (mgr != null)
                {
                    if (Invoke(mgr, "IsFriendlyShown") is bool shown && !shown)
                        return (false, "发现/选择界面尚未展开", responseMode);
                    var waitingMethod = mgr.GetType().GetMethod("IsWaitingToStartChoices", AllInstance, null, new[] { typeof(int) }, null);
                    if (waitingMethod?.Invoke(mgr, new object[] { friendlyPlayerId }) is bool waiting && waiting)
                        return (false, "等待发现界面初始化", responseMode);
                }
                return (true, string.Empty, responseMode);
            }

            if (Invoke(gameState, "IsFriendlySidePlayerTurn") is bool friendlyTurn && !friendlyTurn)
                return (false, "对手回合", responseMode);

            if (!hasLegalActions)
                return (false, "暂无可执行动作", responseMode);

            return (true, string.Empty, responseMode);
        }
        catch
        {
            return (hasPendingChoice || hasLegalActions, string.Empty, responseMode);
        }
    }

    private static bool ToBool(object? value) => value is bool b && b;

    private static void EnsurePlayerEntity(object playerObj, int friendlyPlayerId, Type? gameTagEnum, HashSet<int> choiceEntityIds, Dictionary<int, ProbeEntitySnapshot> entitiesById)
    {
        var snapshot = BuildEntity(playerObj, friendlyPlayerId, gameTagEnum, choiceEntityIds);
        if (snapshot != null && snapshot.Id > 0 && !entitiesById.ContainsKey(snapshot.Id))
            entitiesById[snapshot.Id] = snapshot;
    }

    private static HashSet<int> CollectFriendlyChoiceEntityIds(object gameState)
    {
        var ids = new HashSet<int>();
        try
        {
            var choices = Invoke(gameState, "GetFriendlyEntityChoices");
            if (choices != null && GetFieldOrProperty(choices, "Entities") is IEnumerable list)
            {
                foreach (var item in list)
                {
                    var id = ToInt(item);
                    if (id > 0) ids.Add(id);
                }
            }
        }
        catch { }
        return ids;
    }

    private static ProbePendingChoiceSnapshot? BuildPendingChoice(object gameState, Dictionary<int, ProbeEntitySnapshot> entitiesById)
    {
        try
        {
            var choices = Invoke(gameState, "GetFriendlyEntityChoices");
            if (choices == null)
                return null;

            var choiceId = ToInt(GetFieldOrProperty(choices, "ID"));
            var choiceType = Convert.ToString(GetFieldOrProperty(choices, "ChoiceType"), CultureInfo.InvariantCulture) ?? "GENERAL";
            var sourceId = ToInt(GetFieldOrProperty(choices, "Source"));
            var countMin = ToInt(GetFieldOrProperty(choices, "CountMin"));
            var countMax = ToInt(GetFieldOrProperty(choices, "CountMax"));
            var isSingle = countMax == 0 || (countMin == 1 && countMax == 1);

            var unchoosable = new HashSet<int>();
            if (GetFieldOrProperty(choices, "UnchoosableEntities") is IEnumerable unList)
                foreach (var item in unList) unchoosable.Add(ToInt(item));

            var chosen = new HashSet<int>();
            if (GetFieldOrProperty(gameState, "m_chosenEntities") is IEnumerable chosenList)
                foreach (var item in chosenList) chosen.Add(ExtractEntityId(item));

            var hasConfirm = false;
            try
            {
                var mgrType = FindType("ChoiceCardMgr");
                var mgr = mgrType?.GetMethod("Get", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(null, null);
                if (mgr != null)
                {
                    var confirmBtn = GetFieldOrProperty(mgr, "m_confirmChoiceButton");
                    hasConfirm = confirmBtn != null && !string.Equals(confirmBtn.ToString(), "null", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch { }

            var snapshot = new ProbePendingChoiceSnapshot
            {
                ChoiceId = choiceId,
                ChoiceType = choiceType,
                SourceEntityId = sourceId,
                CountMin = countMin,
                CountMax = countMax,
                IsSingleChoice = isSingle,
                HasConfirmButton = hasConfirm
            };

            if (GetFieldOrProperty(choices, "Entities") is IEnumerable entityIds)
            {
                foreach (var item in entityIds)
                {
                    var id = ToInt(item);
                    if (id <= 0) continue;
                    entitiesById.TryGetValue(id, out var ent);
                    int? atk = null, hp = null;
                    var cost = 0;
                    if (ent != null)
                    {
                        foreach (var tag in ent.Tags)
                        {
                            if (tag.Id == 48) cost = tag.Value;
                            else if (tag.Id == 47) atk = tag.Value;
                            else if (tag.Id == 45) hp = tag.Value;
                        }
                    }
                    snapshot.Cards.Add(new ProbeChoiceCardSnapshot
                    {
                        EntityId = id,
                        CardId = ent?.CardId ?? string.Empty,
                        Name = !string.IsNullOrWhiteSpace(ent?.Name) ? ent!.Name : (ent?.CardId ?? ("#" + id)),
                        CardType = ent?.CardType ?? string.Empty,
                        Cost = cost,
                        Attack = atk,
                        Health = hp,
                        Unchoosable = unchoosable.Contains(id),
                        Chosen = chosen.Contains(id)
                    });
                }
            }

            return snapshot.Cards.Count > 0 ? snapshot : null;
        }
        catch
        {
            return null;
        }
    }

    private static void BuildObservedActions(object gameState, int friendlyPlayerId, Dictionary<int, ProbeEntitySnapshot> entitiesById, List<ProbeObservedActionSnapshot> output)
    {
        try
        {
            var packet = Invoke(gameState, "GetOptionsPacket") ?? GetFieldOrProperty(gameState, "m_options");
            if (packet == null)
                return;

            var listObj = GetFieldOrProperty(packet, "List");
            if (listObj is not IEnumerable optionList)
                return;

            var idx = 0;
            foreach (var option in optionList)
            {
                if (option == null) continue;
                var typeName = Convert.ToString(GetFieldOrProperty(option, "Type"), CultureInfo.InvariantCulture) ?? string.Empty;
                if (string.Equals(typeName, "END_TURN", StringComparison.OrdinalIgnoreCase))
                {
                    output.Add(new ProbeObservedActionSnapshot
                    {
                        Id = $"opt-endturn-{idx++}",
                        Kind = "endTurn",
                        Description = "结束回合"
                    });
                    continue;
                }

                if (!string.Equals(typeName, "POWER", StringComparison.OrdinalIgnoreCase))
                    continue;

                var main = GetFieldOrProperty(option, "Main");
                if (main == null || !IsSubOptionValid(main))
                    continue;

                var sourceId = ToInt(GetFieldOrProperty(main, "ID"));
                if (sourceId <= 0)
                    continue;

                entitiesById.TryGetValue(sourceId, out var src);
                var srcName = !string.IsNullOrWhiteSpace(src?.Name) ? src!.Name : (src?.CardId ?? ("#" + sourceId));
                var kind = ClassifyActionKind(src, friendlyPlayerId);

                var validTargets = new List<int>();
                if (GetFieldOrProperty(main, "Targets") is IEnumerable targets)
                {
                    foreach (var target in targets)
                    {
                        if (target == null || !IsSubOptionValid(target)) continue;
                        var tid = ToInt(GetFieldOrProperty(target, "ID"));
                        if (tid > 0) validTargets.Add(tid);
                    }
                }

                if (validTargets.Count == 0)
                {
                    output.Add(new ProbeObservedActionSnapshot
                    {
                        Id = $"opt-{kind}-{sourceId}-{idx++}",
                        Kind = kind,
                        SourceEntityId = sourceId,
                        Description = srcName
                    });
                }
                else
                {
                    foreach (var tid in validTargets)
                    {
                        entitiesById.TryGetValue(tid, out var tgt);
                        var tgtName = !string.IsNullOrWhiteSpace(tgt?.Name) ? tgt!.Name : (tgt?.CardId ?? ("#" + tid));
                        output.Add(new ProbeObservedActionSnapshot
                        {
                            Id = $"opt-{kind}-{sourceId}-{tid}-{idx++}",
                            Kind = kind,
                            SourceEntityId = sourceId,
                            TargetEntityId = tid,
                            Description = $"{srcName} -> {tgtName}"
                        });
                    }
                }
            }
        }
        catch { }
    }

    private static bool IsSubOptionValid(object subOption)
    {
        var playError = GetFieldOrProperty(subOption, "PlayErrorInfo");
        if (playError == null)
            return true;
        var validObj = Invoke(playError, "IsValid");
        return validObj is bool b ? b : true;
    }

    private static string ClassifyActionKind(ProbeEntitySnapshot? src, int friendlyPlayerId)
    {
        if (src == null) return "playCard";
        if (string.Equals(src.CardType, "HERO_POWER", StringComparison.OrdinalIgnoreCase))
            return "heroPower";
        if (string.Equals(src.Zone, "HAND", StringComparison.OrdinalIgnoreCase))
            return "playCard";
        if (string.Equals(src.Zone, "PLAY", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(src.CardType, "LOCATION", StringComparison.OrdinalIgnoreCase))
            return "activateLocation";
        if (string.Equals(src.Zone, "PLAY", StringComparison.OrdinalIgnoreCase) &&
            src.ControllerId == friendlyPlayerId)
            return "attack";
        return "playCard";
    }

    private static ProbePlayerSnapshot BuildPlayer(object player, string side, bool friendly, Dictionary<int, ProbeEntitySnapshot> entities)
    {
        var playerId = ToInt(Invoke(player, "GetPlayerId") ?? GetFieldOrProperty(player, "PlayerId"));
        var entityId = ToInt(Invoke(player, "GetEntityId") ?? GetFieldOrProperty(player, "EntityId"));
        var hero = Invoke(player, "GetHero") ?? GetFieldOrProperty(player, "Hero");
        var heroPower = Invoke(player, "GetHeroPower") ?? GetFieldOrProperty(player, "HeroPower");
        var weaponCard = Invoke(player, "GetWeaponCard");
        var weapon = weaponCard != null ? Invoke(weaponCard, "GetEntity") : GetFieldOrProperty(player, "Weapon");

        var result = new ProbePlayerSnapshot
        {
            Side = side,
            PlayerId = playerId,
            EntityId = entityId,
            HeroId = ExtractEntityId(hero),
            HeroPowerId = ExtractEntityId(heroPower),
            WeaponId = ExtractEntityId(weapon)
        };

        result.Zones.Add(BuildZone(player, "GetBattlefieldZone", "Battlefield", "PLAY", playerId, true, entities));
        result.Zones.Add(BuildZone(player, "GetHandZone", "Hand", "HAND", playerId, friendly, entities));
        result.Zones.Add(BuildZone(player, "GetDeckZone", "Deck", "DECK", playerId, false, entities));
        result.Zones.Add(BuildSecretZone(player, playerId, friendly, entities));
        return result;
    }

    private static ProbeZoneSnapshot BuildZone(
        object player,
        string zoneGetter,
        string zoneName,
        string zoneTagText,
        int playerId,
        bool exposeEntities,
        Dictionary<int, ProbeEntitySnapshot> entities)
    {
        var zone = Invoke(player, zoneGetter);
        var ids = new List<int>();
        int? explicitCount = null;

        if (zone != null)
        {
            var countObj = Invoke(zone, "GetCardCount") ?? GetFieldOrProperty(zone, "CardCount");
            if (countObj != null)
                explicitCount = ToInt(countObj);

            if (Invoke(zone, "GetCards") is IEnumerable cards)
            {
                foreach (var card in cards)
                {
                    var entity = card != null ? (Invoke(card, "GetEntity") ?? card) : null;
                    var id = ExtractEntityId(entity);
                    if (id > 0)
                        ids.Add(id);
                }
            }
        }

        if (ids.Count == 0)
        {
            foreach (var entity in entities.Values
                .Where(e => e.ControllerId == playerId && string.Equals(e.Zone, zoneTagText, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.ZonePosition <= 0 ? int.MaxValue : e.ZonePosition)
                .ThenBy(e => e.Id))
            {
                ids.Add(entity.Id);
            }
        }

        var count = explicitCount ?? ids.Count;
        var result = new ProbeZoneSnapshot
        {
            Name = zoneName,
            Count = count,
            HiddenCount = exposeEntities ? Math.Max(0, count - ids.Count) : count
        };
        if (exposeEntities)
            result.EntityIds.AddRange(ids);
        return result;
    }

    private static ProbeZoneSnapshot BuildSecretZone(object player, int playerId, bool friendly, Dictionary<int, ProbeEntitySnapshot> entities)
    {
        var secretZone = Invoke(player, "GetSecretZone");
        var ids = new List<int>();
        if (secretZone != null && Invoke(secretZone, "GetSecretCards") is IEnumerable cards)
        {
            foreach (var card in cards)
            {
                var entity = card != null ? (Invoke(card, "GetEntity") ?? card) : null;
                var id = ExtractEntityId(entity);
                if (id > 0)
                    ids.Add(id);
            }
        }

        if (ids.Count == 0)
        {
            foreach (var entity in entities.Values
                .Where(e => e.ControllerId == playerId && string.Equals(e.Zone, "SECRET", StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.ZonePosition <= 0 ? int.MaxValue : e.ZonePosition)
                .ThenBy(e => e.Id))
            {
                ids.Add(entity.Id);
            }
        }

        var result = new ProbeZoneSnapshot
        {
            Name = "Secrets",
            Count = ids.Count,
            HiddenCount = friendly ? 0 : ids.Count
        };
        if (friendly)
            result.EntityIds.AddRange(ids);
        return result;
    }

    private static ProbeEntitySnapshot? BuildEntity(object entity, int friendlyPlayerId, Type? gameTagEnum, HashSet<int> choiceEntityIds)
    {
        var id = ExtractEntityId(entity);
        if (id <= 0)
            return null;

        var rawTags = ExtractTagMap(Invoke(entity, "GetTags") ?? GetFieldOrProperty(entity, "m_tags") ?? GetFieldOrProperty(entity, "Tags"));
        var dormantTags = ExtractTagMap(GetFieldOrProperty(entity, "m_cachedTagsForDormant") ?? GetFieldOrProperty(entity, "CachedTagsForDormant"));

        var controllerId = ToInt(Invoke(entity, "GetControllerId") ?? GetFieldOrProperty(entity, "ControllerId"));
        if (controllerId == 0 && rawTags.TryGetValue(50, out var controllerTag))
            controllerId = controllerTag;

        var zoneText = ConvertZone(Invoke(entity, "GetZone") ?? GetFieldOrProperty(entity, "Zone"), rawTags);
        var zonePos = ToInt(Invoke(entity, "GetZonePosition") ?? GetFieldOrProperty(entity, "ZonePosition"));
        if (zonePos == 0 && rawTags.TryGetValue(263, out var posTag))
            zonePos = posTag;

        var cardTypeText = ConvertCardType(Invoke(entity, "GetCardType") ?? GetFieldOrProperty(entity, "CardType"), rawTags);
        var cardId = Convert.ToString(Invoke(entity, "GetCardId") ?? GetFieldOrProperty(entity, "CardId"), CultureInfo.InvariantCulture) ?? string.Empty;
        var cardName = Convert.ToString(Invoke(entity, "GetName") ?? GetFieldOrProperty(entity, "Name"), CultureInfo.InvariantCulture) ?? string.Empty;

        var isPrivate = !choiceEntityIds.Contains(id) && IsHiddenEntity(zoneText, controllerId, friendlyPlayerId);
        var snapshot = new ProbeEntitySnapshot
        {
            Id = id,
            CardId = isPrivate ? string.Empty : cardId,
            Name = isPrivate ? string.Empty : cardName,
            ControllerId = controllerId,
            Zone = zoneText,
            ZonePosition = zonePos,
            CardType = isPrivate ? string.Empty : cardTypeText
        };

        if (!isPrivate)
        {
            foreach (var pair in rawTags.OrderBy(p => p.Key))
                snapshot.Tags.Add(BuildTag(pair.Key, pair.Value, gameTagEnum));

            foreach (var pair in dormantTags.OrderBy(p => p.Key))
                snapshot.CachedTagsForDormant.Add(BuildTag(pair.Key, pair.Value, gameTagEnum));
        }

        return snapshot;
    }

    private static bool IsHiddenEntity(string zone, int controllerId, int friendlyPlayerId)
    {
        if (string.Equals(zone, "DECK", StringComparison.OrdinalIgnoreCase))
            return true;
        if (controllerId != friendlyPlayerId &&
            (string.Equals(zone, "HAND", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(zone, "SECRET", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(zone, "SETASIDE", StringComparison.OrdinalIgnoreCase)))
            return true;
        return false;
    }

    private static ProbeTagSnapshot BuildTag(int tagId, int tagValue, Type? gameTagEnum)
    {
        string? name = null;
        if (gameTagEnum != null && gameTagEnum.IsEnum)
        {
            try { name = Enum.GetName(gameTagEnum, tagId); }
            catch { name = null; }
        }
        return new ProbeTagSnapshot
        {
            Id = tagId,
            Name = string.IsNullOrEmpty(name) ? "TAG_" + tagId.ToString(CultureInfo.InvariantCulture) : name!,
            Value = tagValue
        };
    }

    private static IEnumerable<object> EnumerateEntityMap(object entityMapObj)
    {
        var list = new List<object>();
        try
        {
            if (entityMapObj is IDictionary dict)
            {
                foreach (DictionaryEntry entry in dict)
                    if (entry.Value != null) list.Add(entry.Value);
                return list;
            }

            if (GetFieldOrProperty(entityMapObj, "Values") is IEnumerable values)
            {
                foreach (var val in values)
                    if (val != null) list.Add(val);
                return list;
            }

            if (entityMapObj is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item == null) continue;
                    var val = GetFieldOrProperty(item, "Value");
                    list.Add(val ?? item);
                }
            }
        }
        catch
        {
            // Ignore concurrent collection modification during a game frame; next tick will retry.
        }
        return list;
    }

    private static Dictionary<int, int> ExtractTagMap(object? tagSource)
    {
        var result = new Dictionary<int, int>();
        if (tagSource == null)
            return result;

        var mapObj = Invoke(tagSource, "GetMap") ?? GetFieldOrProperty(tagSource, "m_values") ?? tagSource;
        try
        {
            if (mapObj is IDictionary dictionary)
            {
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (entry.Key != null && entry.Value != null)
                        result[ToInt(entry.Key)] = ToInt(entry.Value);
                }
            }
            else if (mapObj is IEnumerable enumerable)
            {
                foreach (var entry in enumerable)
                {
                    if (entry == null) continue;
                    var key = GetFieldOrProperty(entry, "Key");
                    var val = GetFieldOrProperty(entry, "Value");
                    if (key != null && val != null)
                        result[ToInt(key)] = ToInt(val);
                }
            }
        }
        catch { }
        return result;
    }

    private static string ConvertZone(object? zoneValue, Dictionary<int, int> tags)
    {
        if (zoneValue != null)
        {
            if (zoneValue is int intZone)
                return ZoneName(intZone);
            var text = zoneValue.ToString();
            if (!string.IsNullOrWhiteSpace(text) && !int.TryParse(text, out _))
                return text!;
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                return ZoneName(parsed);
        }
        return tags.TryGetValue(49, out var tagZone) ? ZoneName(tagZone) : "INVALID";
    }

    private static string ZoneName(int zone) => zone switch
    {
        1 => "PLAY",
        2 => "DECK",
        3 => "HAND",
        4 => "GRAVEYARD",
        5 => "REMOVEDFROMGAME",
        6 => "SETASIDE",
        7 => "SECRET",
        8 => "LETTUCE_ABILITY",
        _ => "INVALID"
    };

    private static string ConvertCardType(object? cardTypeValue, Dictionary<int, int> tags)
    {
        if (cardTypeValue != null)
        {
            if (cardTypeValue is int intType)
                return CardTypeName(intType);
            var text = cardTypeValue.ToString();
            if (!string.IsNullOrWhiteSpace(text) && !int.TryParse(text, out _))
                return text!;
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                return CardTypeName(parsed);
        }
        return tags.TryGetValue(202, out var tagType) ? CardTypeName(tagType) : "INVALID";
    }

    private static string CardTypeName(int cardType) => cardType switch
    {
        1 => "GAME",
        2 => "PLAYER",
        3 => "HERO",
        4 => "MINION",
        5 => "SPELL",
        6 => "ENCHANTMENT",
        7 => "WEAPON",
        8 => "ITEM",
        9 => "TOKEN",
        10 => "HERO_POWER",
        22 => "LOCATION",
        _ => "INVALID"
    };

    private static int ExtractEntityId(object? entity) =>
        entity == null ? 0 : ToInt(Invoke(entity, "GetEntityId") ?? GetFieldOrProperty(entity, "EntityId") ?? GetFieldOrProperty(entity, "Id"));

    private static int ComputeFingerprint(ProbeFrameSnapshot frame)
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 31) + frame.Match.FriendlyPlayerId;
            hash = (hash * 31) + frame.Match.OpponentPlayerId;
            foreach (var entity in frame.Entities)
            {
                hash = (hash * 31) + entity.Id;
                hash = (hash * 31) + (entity.CardId?.GetHashCode() ?? 0);
                hash = (hash * 31) + entity.ControllerId;
                hash = (hash * 31) + (entity.Zone?.GetHashCode() ?? 0);
                hash = (hash * 31) + entity.ZonePosition;
                foreach (var tag in entity.Tags)
                {
                    hash = (hash * 31) + tag.Id;
                    hash = (hash * 31) + tag.Value;
                }
                foreach (var dtag in entity.CachedTagsForDormant)
                {
                    hash = (hash * 31) + dtag.Id;
                    hash = (hash * 31) + dtag.Value;
                }
            }
            if (frame.PendingChoice != null)
            {
                hash = (hash * 31) + frame.PendingChoice.ChoiceId;
                hash = (hash * 31) + frame.PendingChoice.Cards.Count;
                hash = (hash * 31) + (frame.PendingChoice.HasConfirmButton ? 1 : 0);
                foreach (var card in frame.PendingChoice.Cards)
                {
                    hash = (hash * 31) + card.EntityId;
                    hash = (hash * 31) + (card.Chosen ? 1 : 0);
                }
            }
            hash = (hash * 31) + frame.LegalActions.Count;
            hash = (hash * 31) + (frame.CanAct ? 1 : 0);
            hash = (hash * 31) + (frame.BlockReason?.GetHashCode() ?? 0);
            foreach (var action in frame.LegalActions)
            {
                hash = (hash * 31) + (action.SourceEntityId ?? 0);
                hash = (hash * 31) + (action.TargetEntityId ?? 0);
            }
            return hash;
        }
    }

    public static string SerializeJson(ProbeFrameSnapshot frame)
    {
        var sb = new StringBuilder(8192);
        sb.Append("{\"protocolVersion\":").Append(frame.ProtocolVersion.ToString(CultureInfo.InvariantCulture))
          .Append(",\"revision\":").Append(frame.Revision.ToString(CultureInfo.InvariantCulture))
          .Append(",\"capturedAtUtc\":\"").Append(Escape(frame.CapturedAtUtc)).Append('"')
          .Append(",\"canAct\":").Append(frame.CanAct ? "true" : "false")
          .Append(",\"blockReason\":\"").Append(Escape(frame.BlockReason)).Append('"')
          .Append(",\"responseMode\":\"").Append(Escape(frame.ResponseMode)).Append('"')
          .Append(",\"match\":{\"gameActive\":").Append(frame.Match.GameActive ? "true" : "false")
          .Append(",\"friendlyPlayerId\":").Append(frame.Match.FriendlyPlayerId.ToString(CultureInfo.InvariantCulture))
          .Append(",\"opponentPlayerId\":").Append(frame.Match.OpponentPlayerId.ToString(CultureInfo.InvariantCulture))
          .Append(",\"friendlyEntityId\":").Append(frame.Match.FriendlyEntityId.ToString(CultureInfo.InvariantCulture))
          .Append(",\"opponentEntityId\":").Append(frame.Match.OpponentEntityId.ToString(CultureInfo.InvariantCulture))
          .Append(",\"players\":[");

        for (var i = 0; i < frame.Match.Players.Count; i++)
        {
            if (i > 0) sb.Append(',');
            var p = frame.Match.Players[i];
            sb.Append("{\"side\":\"").Append(Escape(p.Side)).Append('"')
              .Append(",\"entityId\":").Append(p.EntityId.ToString(CultureInfo.InvariantCulture))
              .Append(",\"playerId\":").Append(p.PlayerId.ToString(CultureInfo.InvariantCulture))
              .Append(",\"heroId\":").Append(p.HeroId.ToString(CultureInfo.InvariantCulture))
              .Append(",\"heroPowerId\":").Append(p.HeroPowerId.ToString(CultureInfo.InvariantCulture))
              .Append(",\"weaponId\":").Append(p.WeaponId.ToString(CultureInfo.InvariantCulture))
              .Append(",\"zones\":[");
            for (var z = 0; z < p.Zones.Count; z++)
            {
                if (z > 0) sb.Append(',');
                var zone = p.Zones[z];
                sb.Append("{\"name\":\"").Append(Escape(zone.Name)).Append('"')
                  .Append(",\"count\":").Append(zone.Count.ToString(CultureInfo.InvariantCulture))
                  .Append(",\"hiddenCount\":").Append(zone.HiddenCount.ToString(CultureInfo.InvariantCulture))
                  .Append(",\"entityIds\":[");
                for (var e = 0; e < zone.EntityIds.Count; e++)
                {
                    if (e > 0) sb.Append(',');
                    sb.Append(zone.EntityIds[e].ToString(CultureInfo.InvariantCulture));
                }
                sb.Append("]}");
            }
            sb.Append("]}");
        }

        sb.Append("]},\"entities\":[");
        for (var i = 0; i < frame.Entities.Count; i++)
        {
            if (i > 0) sb.Append(',');
            var entity = frame.Entities[i];
            sb.Append("{\"id\":").Append(entity.Id.ToString(CultureInfo.InvariantCulture))
              .Append(",\"cardId\":\"").Append(Escape(entity.CardId)).Append('"')
              .Append(",\"name\":\"").Append(Escape(entity.Name)).Append('"')
              .Append(",\"controllerId\":").Append(entity.ControllerId.ToString(CultureInfo.InvariantCulture))
              .Append(",\"zone\":\"").Append(Escape(entity.Zone)).Append('"')
              .Append(",\"zonePosition\":").Append(entity.ZonePosition.ToString(CultureInfo.InvariantCulture))
              .Append(",\"cardType\":\"").Append(Escape(entity.CardType)).Append('"')
              .Append(",\"tags\":[");
            AppendTags(sb, entity.Tags);
            sb.Append("],\"cachedTagsForDormant\":[");
            AppendTags(sb, entity.CachedTagsForDormant);
            sb.Append("]}");
        }
        sb.Append(']');
        if (frame.PendingChoice != null)
        {
            var pc = frame.PendingChoice;
            sb.Append(",\"pendingChoice\":{\"choiceId\":").Append(pc.ChoiceId.ToString(CultureInfo.InvariantCulture))
              .Append(",\"choiceType\":\"").Append(Escape(pc.ChoiceType)).Append('"')
              .Append(",\"sourceEntityId\":").Append(pc.SourceEntityId.ToString(CultureInfo.InvariantCulture))
              .Append(",\"countMin\":").Append(pc.CountMin.ToString(CultureInfo.InvariantCulture))
              .Append(",\"countMax\":").Append(pc.CountMax.ToString(CultureInfo.InvariantCulture))
              .Append(",\"isSingleChoice\":").Append(pc.IsSingleChoice ? "true" : "false")
              .Append(",\"hasConfirmButton\":").Append(pc.HasConfirmButton ? "true" : "false")
              .Append(",\"cards\":[");
            for (var c = 0; c < pc.Cards.Count; c++)
            {
                if (c > 0) sb.Append(',');
                var card = pc.Cards[c];
                sb.Append("{\"entityId\":").Append(card.EntityId.ToString(CultureInfo.InvariantCulture))
                  .Append(",\"cardId\":\"").Append(Escape(card.CardId)).Append('"')
                  .Append(",\"name\":\"").Append(Escape(card.Name)).Append('"')
                  .Append(",\"cardType\":\"").Append(Escape(card.CardType)).Append('"')
                  .Append(",\"cost\":").Append(card.Cost.ToString(CultureInfo.InvariantCulture))
                  .Append(",\"attack\":").Append(card.Attack.HasValue ? card.Attack.Value.ToString(CultureInfo.InvariantCulture) : "null")
                  .Append(",\"health\":").Append(card.Health.HasValue ? card.Health.Value.ToString(CultureInfo.InvariantCulture) : "null")
                  .Append(",\"unchoosable\":").Append(card.Unchoosable ? "true" : "false")
                  .Append(",\"chosen\":").Append(card.Chosen ? "true" : "false")
                  .Append('}');
            }
            sb.Append("]}");
        }
        if (frame.LegalActions.Count > 0)
        {
            sb.Append(",\"legalActions\":[");
            for (var a = 0; a < frame.LegalActions.Count; a++)
            {
                if (a > 0) sb.Append(',');
                var act = frame.LegalActions[a];
                sb.Append("{\"id\":\"").Append(Escape(act.Id)).Append('"')
                  .Append(",\"kind\":\"").Append(Escape(act.Kind)).Append('"');
                if (act.SourceEntityId.HasValue)
                    sb.Append(",\"sourceEntityId\":").Append(act.SourceEntityId.Value.ToString(CultureInfo.InvariantCulture));
                if (act.TargetEntityId.HasValue)
                    sb.Append(",\"targetEntityId\":").Append(act.TargetEntityId.Value.ToString(CultureInfo.InvariantCulture));
                if (act.Position.HasValue)
                    sb.Append(",\"position\":").Append(act.Position.Value.ToString(CultureInfo.InvariantCulture));
                sb.Append(",\"description\":\"").Append(Escape(act.Description)).Append("\"}");
            }
            sb.Append(']');
        }
        sb.Append('}');
        return sb.ToString();
    }

    private static void AppendTags(StringBuilder sb, List<ProbeTagSnapshot> tags)
    {
        for (var t = 0; t < tags.Count; t++)
        {
            if (t > 0) sb.Append(',');
            var tag = tags[t];
            sb.Append("{\"id\":").Append(tag.Id.ToString(CultureInfo.InvariantCulture))
              .Append(",\"name\":\"").Append(Escape(tag.Name)).Append('"')
              .Append(",\"value\":").Append(tag.Value.ToString(CultureInfo.InvariantCulture))
              .Append('}');
        }
    }

    private static string Escape(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        return text!.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    internal static Type? FindGameStateType()
    {
        Type? fallback = null;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var type = assembly.GetType("GameState", throwOnError: false);
                if (type != null)
                {
                    if (type.GetMethod("GetEntityMap", AllInstance) != null)
                        return type;
                    fallback ??= type;
                }
            }
            catch { }
        }
        return fallback;
    }

    internal static Type? FindType(string simpleName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var type = assembly.GetType(simpleName, throwOnError: false);
                if (type != null)
                    return type;
            }
            catch { }
        }
        return null;
    }

    internal static object? Invoke(object target, string methodName)
    {
        var method = target.GetType().GetMethod(methodName, AllInstance, null, Type.EmptyTypes, null);
        return method?.Invoke(target, null);
    }

    internal static object? GetFieldOrProperty(object target, string memberName)
    {
        var type = target.GetType();
        var prop = type.GetProperty(memberName, AllInstance);
        if (prop != null)
            return prop.GetValue(target, null);
        var field = type.GetField(memberName, AllInstance);
        return field?.GetValue(target);
    }

    private static int ToInt(object? value)
    {
        if (value == null)
            return 0;
        if (value is int i)
            return i;
        if (value is Enum)
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        return int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed : 0;
    }
}
