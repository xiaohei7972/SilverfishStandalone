using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    /// <summary>
    /// Preview known, explicit CHOICE effects in a separate immutable branch.
    /// This is NOT Apply: the authoritative pending packet stays submitted.
    /// Random effects are enumerated exactly when possible, then sampled.
    /// </summary>
    public SimulationDistribution PreviewChoiceDistribution(
        GameState state, SimulationAction action, ChoicePreviewRules rules, int workLimit = 64)
    {
        ValidateWorkLimit(workLimit);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(rules);
        var packet = state.PendingChoice?.Packet ??
            throw new InvalidOperationException("没有待决选择包可用于离线预演。");
        ValidateChoicePreviewRules(state, packet, rules);
        // Reuse all normal CHOICE submission and stale-packet checks without
        // applying any game effects to the real input state.
        var acknowledgement = Apply(state, action);
        return RunDistributionCore(randomChoice =>
            new Resolution(this, acknowledgement.State, randomChoice).RunChoicePreview(
                packet, acknowledgement.State.PendingChoice!.Submitted!, rules, acknowledgement.Events),
            workLimit);
    }

    /// <summary>
    /// Only returns a single-state preview if its probability is known exactly.
    /// Use PreviewChoiceDistribution for random outcomes.
    /// </summary>
    public ChoicePreviewResult PreviewChoice(
        GameState state, SimulationAction action, ChoicePreviewRules rules, int workLimit = 64)
    {
        var distribution = PreviewChoiceDistribution(state, action, rules, workLimit);
        if (!distribution.IsExact || distribution.Outcomes is not [var only] || only.Probability != 1.0)
            throw new InvalidOperationException("选择效果存在多个随机结果；请使用 PreviewChoiceDistribution。");
        return new(only.Result.State, only.Result.Events);
    }

    private void ValidateChoicePreviewRules(
        GameState state, EntityChoicePacket packet, ChoicePreviewRules rules)
    {
        if (rules.ChoiceId != packet.ChoiceId || rules.Type != packet.Type ||
            rules.Type == EntityChoiceType.Mulligan)
            throw new ArgumentException("离线规则的包 ID 或类型不匹配，或尚不支持该选择类型。", nameof(rules));
        if (packet.Source is not EntityId sourceId ||
            !state.Entities.TryGetValue(sourceId, out var source) ||
            source.Controller != packet.Player || !_cards.TryGetValue(source.CardId, out var sourceCard))
            throw new ArgumentException("缺少属于选择者的已知来源卡牌，不能推测选择效果。", nameof(rules));

        if (rules.Type == EntityChoiceType.Target)
        {
            if (rules.PerTargetEffects.IsDefaultOrEmpty || rules.PerOptionEffects is null ||
                !rules.PerOptionEffects.IsEmpty)
                throw new ArgumentException("TARGET 选择必须有明确的逐目标效果，且不能混入发现选项。", nameof(rules));
            var targets = packet.Entities.Except(packet.UnchoosableEntities).ToArray();
            if (targets.Any(id => !state.Entities.TryGetValue(id, out var target) ||
                target.Character is not { Health: > 0 } || state.LocationOf(id).Zone != GameZone.Play))
                throw new ArgumentException("TARGET 候选目标必须是当前局面中存活的场上角色。", nameof(rules));

            var requirement = targets.All(id => Card(state.Entities[id]).Definition.Kind == EntityKind.Minion)
                ? TargetRequirement.Minion : TargetRequirement.Character;
            ValidateEffects(sourceCard, rules.PerTargetEffects, requirement);
            return;
        }

        if (!rules.PerTargetEffects.IsEmpty || rules.PerTargetEffects.IsDefault ||
            rules.PerOptionEffects is null ||
            rules.PerOptionEffects.Keys.Any(id => !packet.Entities.Contains(id)) ||
            packet.Entities.Except(packet.UnchoosableEntities).Any(id =>
                !rules.PerOptionEffects.TryGetValue(id, out var effects) || effects.IsDefault))
            throw new ArgumentException("GENERAL 预演必须为每个可选实体明确提供效果，且不得使用逐目标效果。", nameof(rules));

        foreach (var effects in rules.PerOptionEffects.Values)
            ValidateEffects(sourceCard, effects);
    }

    private sealed partial class Resolution
    {
        internal SimulationResult RunChoicePreview(
            EntityChoicePacket packet, EntityChoiceSubmission submission, ChoicePreviewRules rules,
            ImmutableArray<SimulationEvent> acknowledgement)
        {
            _events.AddRange(acknowledgement);
            var source = packet.Source!.Value;
            foreach (var selected in submission.OrderedEntityIds)
            {
                if (_state.Outcome != MatchOutcome.InProgress)
                    break;
                var effects = rules.Type == EntityChoiceType.Target
                    ? rules.PerTargetEffects : rules.PerOptionEffects[selected];
                var target = rules.Type == EntityChoiceType.Target ? selected : (EntityId?)null;
                ResolveEffects(new(source, packet.Player, target), effects);
                ResolveDeaths();
                FinishIfHeroesDied();
            }
            return new(_state, _events.ToImmutable());
        }
    }
}
