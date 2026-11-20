using System.Collections.Immutable;
using Silverfish.Domain;

namespace Silverfish.Simulation;

public sealed partial class GameSimulator
{
    /// <summary>计算指定手牌或英雄技能在当前局面下的实际费用。</summary>
    public int EffectiveCost(GameState state, EntityId id)
    {
        if (state is null)
            throw new ArgumentNullException(nameof(state), "必须提供对局状态。");
        if (!state.Entities.TryGetValue(id, out var entity) ||
            state.LocationOf(id).Zone is not (GameZone.Hand or GameZone.HeroPower))
            throw new ArgumentException($"实体 {id} 必须是手牌或英雄技能，才能查询当前费用。", nameof(id));

        return EffectiveCost(entity, Card(entity), ActiveCostAuras(state, entity.Controller));
    }

    private ImmutableArray<CostAura> ActiveCostAuras(GameState state, PlayerId recipient)
    {
        if (!_hasCostAuras)
            return [];

        var applicable = ImmutableArray.CreateBuilder<CostAura>();
        foreach (var player in state.Players.Values)
        {
            var relation = player.Id == recipient ? CostAuraRecipient.Friendly : CostAuraRecipient.Opponent;
            foreach (var id in player.In(GameZone.Play))
            {
                var source = state.Entities[id];
                if (source.Silenced ||
                    (source.Character is not { Health: > 0 } &&
                        (player.EquippedWeapon != id || source.Weapon is not { Durability: > 0 })))
                    continue;

                foreach (var aura in Card(source).CostAuras)
                    if (aura.Recipient == relation || aura.Recipient == CostAuraRecipient.BothPlayers)
                        applicable.Add(aura);
            }
        }
        return applicable.ToImmutable();
    }

    private static int EffectiveCost(GameEntity entity, SimulationCard card, ImmutableArray<CostAura> auras)
    {
        long cost = card.Definition.PrintedCost;
        foreach (var modifier in entity.CostModifiers)
            cost += modifier.Delta;

        if (!auras.IsEmpty)
        {
            var target = card.Definition.Kind switch
            {
                EntityKind.Minion => CostAuraTarget.Minion,
                EntityKind.Spell => CostAuraTarget.Spell,
                EntityKind.Weapon => CostAuraTarget.Weapon,
                EntityKind.HeroPower => CostAuraTarget.HeroPower,
                _ => throw new InvalidOperationException("该实体类型不支持计算使用费用。")
            };
            foreach (var aura in auras)
                if (aura.Target == target || (aura.Target == CostAuraTarget.Hand && target != CostAuraTarget.HeroPower))
                    cost += aura.Delta;
        }

        // 把单卡修正和场上光环全部相加后，再限制实际费用的范围。
        return (int)Math.Clamp(cost, 0, int.MaxValue);
    }
}
