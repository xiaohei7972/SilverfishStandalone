using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 载人飞天魔像
// Piloted Sky Golem
// 亡语：随机召唤一个法力值消耗为（4）的随从。
// Deathrattle: Summon a random 4-Cost minion.
[CardSim("GVG_105")]
internal static class Sim_GVG_105
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonMatchingMinionEffect(
                new CardFilter
                {
                    Kind = EntityKind.Minion,
                    MinimumPrintedCost = 4,
                    MaximumPrintedCost = 4
                })
        ]
    };
}
