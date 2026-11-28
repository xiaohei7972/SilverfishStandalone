using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 载人收割机
// Piloted Shredder
// 亡语：随机召唤一个法力值消耗为（2）的随从。
// Deathrattle: Summon a random 2-Cost minion.
[CardSim("GVG_096")]
internal static class Sim_GVG_096
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonMatchingMinionEffect(
                new CardFilter
                {
                    Kind = EntityKind.Minion,
                    MinimumPrintedCost = 2,
                    MaximumPrintedCost = 2
                })
        ]
    };
}
