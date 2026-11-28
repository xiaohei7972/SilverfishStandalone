using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 无面召唤者
// Faceless Summoner
// 战吼：随机召唤一个法力值消耗为（3）的随从。
// Battlecry: Summon a random 3-Cost minion.
[CardSim("OG_207")]
internal static class Sim_OG_207
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonMatchingMinionEffect(new CardFilter { Kind = EntityKind.Minion, MinimumPrintedCost = 3, MaximumPrintedCost = 3 }),
        ],
    };
}
