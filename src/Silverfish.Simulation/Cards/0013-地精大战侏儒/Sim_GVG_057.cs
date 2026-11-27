using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 光明圣印
// Seal of Light
// 为你的英雄恢复4点生命值，并在本回合中 获得+2攻击力。
// Restore 4 Health to your hero and gain +2 Attack this turn.
[CardSim("GVG_057")]
internal static class Sim_GVG_057
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new HealEffect(HealTarget.FriendlyHero, 4),
            new EnchantStatsEffect(
                StatTarget.FriendlyHero,
                new(StatModifierMode.Add, 2),
                EnchantmentDuration.CurrentTurn)
            {
                EnchantmentCardId = Sim_GVG_057a.CardId
            }
        ]
    };
}
