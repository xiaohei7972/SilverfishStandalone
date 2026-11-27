using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 斯尼德的伐木机
// Sneed's Old Shredder
// 亡语：&随机召唤一个传说随从。
// Deathrattle:& Summon a random Legendary minion.
[CardSim("GVG_114")]
internal static class Sim_GVG_114
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new SummonMatchingMinionEffect(new CardFilter { Kind = EntityKind.Minion, IsLegendary = true })
        ]
    };
}
