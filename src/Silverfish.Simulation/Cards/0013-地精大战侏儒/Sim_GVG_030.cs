using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 电镀机械熊仔
// Anodized Robo Cub
// 嘲讽，抉择： +1攻击力；或者+1生命值。
// Taunt. Choose One - +1 Attack; or +1 Health.
[CardSim("GVG_030")]
internal static class Sim_GVG_030
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new(
                "GVG_030a",
                [
                    new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, Attack: 1))
                    {
                        EnchantmentCardId = Sim_GVG_030ae.CardId
                    }
                ]),
            new(
                "GVG_030b",
                [
                    new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, MaximumHealth: 1))
                    {
                        EnchantmentCardId = Sim_GVG_030be.CardId
                    }
                ])
        ]
    };
}
