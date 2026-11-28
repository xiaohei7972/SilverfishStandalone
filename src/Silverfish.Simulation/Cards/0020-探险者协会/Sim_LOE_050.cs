using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 骑乘迅猛龙
// Mounted Raptor
// 亡语：随机召唤一个法力值消耗为（1）的随从。
// Deathrattle: Summon a random 1-Cost minion.
[CardSim("LOE_050")]
internal static class Sim_LOE_050
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonMatchingMinionEffect(new CardFilter { Kind = EntityKind.Minion, MinimumPrintedCost = 1, MaximumPrintedCost = 1 }),
        ],
    };
}
