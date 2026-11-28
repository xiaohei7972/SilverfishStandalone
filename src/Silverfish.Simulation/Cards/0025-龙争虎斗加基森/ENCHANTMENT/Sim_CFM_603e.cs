using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 疯狂药水
// Madness Potion
// 在本回合中，本随从的控制权发生了变化。
// This minion has switched controllers this turn.
internal static class Sim_CFM_603e
{
    internal const string CardId = "CFM_603e";
    internal static StatEnchantmentRule Rule { get; } =
        new(CardId, new(StatModifierMode.Add, Attack: 0), EnchantmentDuration.CurrentTurn)
        {
            OnRemoved =
            [
                new(EnchantmentRemovalReason.Expired,
                    [new TakeControlEffect(EffectTarget.TriggerSubject, ReturnToSourceOpponent: true)]),
            ],
        };
}
