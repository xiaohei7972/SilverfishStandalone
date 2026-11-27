using System.Collections.Immutable;
using Silverfish.Domain;
using Silverfish.Domain.Client;
using static Silverfish.Domain.Client.ErrorType;

namespace Silverfish.Simulation;

public sealed record SourceExplanation(EntityId Source, string? CardId, bool EffectImplemented,
    ImmutableArray<PlayBranch> Branches, PlayFailure? Failure);

public sealed partial class GameSimulator
{
    public SourceExplanation ExplainSource(GameState state, EntityId id)
    {
        if (!state.Entities.TryGetValue(id, out var entity))
            return new(id, null, false, [], new("来源实体不存在", Source: id));
        var card = Card(entity);
        if (state.PendingChoice is not null)
            return new(id, entity.CardId, !card.EffectPending, [],
                new("正在等待客户端 CHOICE 包完成并刷新权威局面", Source: id));
        if (state.PendingPlayChoice is { } choice)
            return id == choice.Source
                ? new(id, entity.CardId, true, [new(null, PlayTargetMode.Required,
                    [.. choice.Options.Select(option => (EntityId?)option)], null)], null)
                : new(id, entity.CardId, !card.EffectPending, [], new("请先完成待决选择", Source: id));
        var zone = state.LocationOf(id).Zone;
        var ability = zone == GameZone.Play && card.Definition.Kind == EntityKind.Location;
        var branches = Branches(state, entity, card, ability).ToImmutableArray();
        var failure = SourceFailure(state, entity, card, zone, ActiveCostAuras(state, entity.Controller));
        return new(id, entity.CardId, !card.EffectPending, branches, failure);
    }

    private IEnumerable<PlayBranch> Branches(GameState state, GameEntity entity, SimulationCard card, bool activation)
    {
        if (activation)
            yield return EvaluateBranch(state, entity, card, ability: card.Activation ?? new([]));
        else if (card.Choices.IsEmpty)
            yield return EvaluateBranch(state, entity, card);
        else
            foreach (var choice in card.Choices)
                yield return EvaluateBranch(state, entity, card, choice);
    }

    private PlayFailure? SourceFailure(GameState state, GameEntity source, SimulationCard card, GameZone zone,
        ImmutableArray<CostAura> costAuras)
    {
        var player = state.Players[source.Controller];
        if (state.Outcome != MatchOutcome.InProgress)
            return new("对局已结束", Source: source.Id);
        if (state.PendingChoice is not null)
            return new("待决 CHOICE 包未完成，不能执行常规动作", Source: source.Id);
        if (state.ActivePlayer != player.Id)
            return new("尚未轮到来源的控制者", REQ_YOUR_TURN, Source: source.Id);
        if (card.EffectPending)
            return new("卡牌效果待实现", Source: source.Id);
        if (card.IsPlayChoiceOption)
            return new("此卡牌只能通过待决选择使用", Source: source.Id);
        if (zone == GameZone.Play && source.Location is { } location)
            return location.TurnsUntilReady > 0 ? new("地标仍在冷却", REQ_NOT_EXHAUSTED_LOCATION, 0, location.TurnsUntilReady, source.Id) : null;
        if (zone is not (GameZone.Hand or GameZone.HeroPower))
            return new("来源不在可使用的区域", Source: source.Id);
        if (zone == GameZone.HeroPower && player.HeroPowerUsesThisTurn > 0)
            return new("英雄技能本回合已使用", REQ_NOT_EXHAUSTED_HERO_POWER, Source: source.Id);
        var type = EffectiveCostType(source, card);
        if (!SupportsCostType(type))
            return new($"卡牌「{source.CardId}」的替代费用类型为 {type}，尚未接入该支付方式。", Source: source.Id);
        var cost = EffectiveCost(state, source, card, costAuras);
        if (!CanPayCardCost(state, player, type, cost))
        {
            var actual = type switch
            {
                TAG_CARD_ALTERNATE_COST.HEALTH => state.Entities[player.Hero].Character!.Health,
                TAG_CARD_ALTERNATE_COST.ARMOR => state.Entities[player.Hero].Character!.Armor,
                TAG_CARD_ALTERNATE_COST.CORPSES => player.AvailableCorpses,
                _ => player.Mana
            };
            return new($"支付资源不足（{type}）", type == TAG_CARD_ALTERNATE_COST.CORPSES ? REQ_MINIMUM_CORPSES : REQ_ENOUGH_MANA,
                cost, actual, source.Id);
        }
        if (zone == GameZone.Hand && UsesBoardSlot(card.Definition.Kind) && BoardSlotCount(state, player.Id) >= BoardLimit)
            return new("没有放置卡牌的空位", REQ_NUM_MINION_SLOTS, 1, 0, source.Id);
        if (card.IsSecret && !CanArmSecret(state, player, source.CardId))
        {
            var secrets = player.In(GameZone.Secret);
            return secrets.Length >= SecretLimit ? new("奥秘区已满", REQ_SECRET_ZONE_CAP, SecretLimit, secrets.Length, source.Id)
                : new("已存在相同奥秘", REQ_UNIQUE_SECRET_OR_QUEST, Source: source.Id);
        }
        return null;
    }

    public PlayFailure? CheckAction(GameState state, SimulationAction action) => CheckAction(state, action, out _);

    private PlayFailure? CheckAction(GameState state, SimulationAction action, out bool resolvesEffects)
    {
        resolvesEffects = false;
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);
        // Client CHOICE is a submission handshake, not a normal legal simulator action.
        // Do not let PlayCard/UseHeroPower/ActivateLocation bypass LegalActions(),
        // which intentionally exposes no ordinary moves while this packet is pending.
        if (state.PendingChoice is not null)
            return new("当前存在客户端 CHOICE 包：只能提交本次选择并等待新快照",
                Source: action.Source, Target: action.Target);
        if (action.ChoicePacketId is not null || !action.OrderedChoiceTargets.IsDefaultOrEmpty)
            return new("普通模拟动作不能携带客户端 CHOICE 包 ID 或有序实体目标",
                Source: action.Source, Target: action.Target);
        if (action.Actor != state.ActivePlayer || state.Outcome != MatchOutcome.InProgress)
            return new("对局已结束或当前不是该玩家的回合", REQ_YOUR_TURN, Source: action.Source);
        if (state.PendingPlayChoice is { } pending)
            return action.Kind == SimulationActionKind.ChooseOption && action.Source == pending.Source &&
                action.Target is EntityId option && pending.Options.Contains(option) && action.Position is null && action.Choice is null
                ? null : new("当前只能从待决选择的候选中选一项", Source: action.Source, Target: action.Target);
        if (action.Kind == SimulationActionKind.ChooseOption)
            return new("当前没有待决选择", Source: action.Source, Target: action.Target);
        if (action.Kind is SimulationActionKind.Attack or SimulationActionKind.EndTurn)
            return LegalActions(state).Contains(action) ? null : new("当前不能执行此动作", Source: action.Source, Target: action.Target);
        if (action.Source is not EntityId id || !state.Entities.TryGetValue(id, out var entity))
            return new("来源实体不存在", Source: action.Source);
        if (action.Target is EntityId target && !state.Entities.ContainsKey(target))
            return new("目标实体不存在", Source: id, Target: target);
        var card = Card(entity);
        var zone = state.LocationOf(id).Zone;
        var kind = zone switch
        {
            GameZone.Hand => SimulationActionKind.PlayCard,
            GameZone.HeroPower => SimulationActionKind.UseHeroPower,
            GameZone.Play when entity.Location is not null => SimulationActionKind.ActivateLocation,
            _ => (SimulationActionKind?)null
        };
        if (action.Kind != kind || entity.Controller != action.Actor)
            return new("动作类型、控制者或来源区域不匹配", Source: id);
        if (SourceFailure(state, entity, card, zone, ActiveCostAuras(state, entity.Controller)) is { } failure)
            return failure;
        var choice = card.Choices.FirstOrDefault(c => c.Id == action.Choice);
        if ((!card.Choices.IsEmpty || action.Choice is not null) && choice is null)
            return new("需要有效的抉择分支", Source: id);
        var branch = EvaluateBranch(state, entity, card, choice,
            kind == SimulationActionKind.ActivateLocation ? card.Activation : null, action.Target);
        if (branch.Failure is not null)
            return branch.Failure;
        if (!branch.Targets.Contains(action.Target))
            return new("此分支必须选取合法目标", REQ_TARGET_TO_PLAY, Source: id, Target: action.Target);
        if (kind == SimulationActionKind.PlayCard && UsesBoardSlot(card.Definition.Kind))
        {
            var max = BoardSlotCount(state, entity.Controller);
            if (action.Position is not int position || position < 0 || position > max)
                return new("落点超出可插入范围", Parameter: max, Actual: action.Position, Source: id);
        }
        else if (action.Position is not null)
            return new("此动作不能指定落点", Source: id, Actual: action.Position);
        resolvesEffects = branch.ResolvesEffects(action.Target);
        return null;
    }
}
