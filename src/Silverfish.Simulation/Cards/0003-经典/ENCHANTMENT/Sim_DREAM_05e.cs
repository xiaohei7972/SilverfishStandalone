using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 梦魇
// Nightmare
// 拥有+5/+5，但它马上会被消灭。
// Has +5/+5, but will be destroyed soon.
internal static class Sim_DREAM_05e
{
    internal static StatEnchantmentRule Rule { get; } =
        new("DREAM_05e", new(StatModifierMode.Add, Attack: 5, MaximumHealth: 5), EnchantmentDuration.UntilNextTurn)
        {
            OnRemoved = [new(EnchantmentRemovalReason.Expired, [new DestroyMinionEffect(EffectTarget.TriggerSubject)])]
        };
}
