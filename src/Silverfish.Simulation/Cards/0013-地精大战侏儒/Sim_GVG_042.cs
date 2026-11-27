using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 耐普图隆
// Neptulon
// 战吼：随机将四张鱼人牌置入你的手牌。过载：（3）
// Battlecry: Add 4 random Murlocs to your hand. Overload: (3)
[CardSim("GVG_042")]
internal static class Sim_GVG_042
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Minion, MinionType = MinionType.Murloc }, Count: 4)
        ]
    };
}
