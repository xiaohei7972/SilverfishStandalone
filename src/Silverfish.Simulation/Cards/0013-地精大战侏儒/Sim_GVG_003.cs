using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 不稳定的传送门
// Unstable Portal
// 随机将一张随从牌置入你的手牌。该牌的法力值消耗减少（3）点。
// Add a random minion to your hand. It costs (3) less.
[CardSim("GVG_003")]
internal static class Sim_GVG_003
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateRandomMatchingCardEffect(
                new CardFilter { Kind = EntityKind.Minion },
                Count: 1,
                CostDelta: -3)
        ]
    };
}
