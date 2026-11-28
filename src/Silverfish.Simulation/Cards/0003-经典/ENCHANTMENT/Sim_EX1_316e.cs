using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 力量的代价
// Power Overwhelming
// 本随从拥有+4/+4，但会在回合结束时惨死。
// This minion has +4/+4, but will die a horrible death at the end of the turn.
internal static class Sim_EX1_316e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("EX1_316e", new(StatModifierMode.Add, Attack: 4, MaximumHealth: 4), EnchantmentDuration.CurrentTurn)
        {
            OnRemoved = [new(EnchantmentRemovalReason.Expired, [new DestroyMinionEffect(EffectTarget.TriggerSubject)])]
        };
}
