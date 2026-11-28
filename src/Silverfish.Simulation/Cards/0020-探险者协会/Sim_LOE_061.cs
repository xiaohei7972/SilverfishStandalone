using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 阿努比萨斯哨兵
// Anubisath Sentinel
// 亡语：随机使一个友方随从获得+3/+3。
// Deathrattle: Give a random friendly minion +3/+3.
[CardSim("LOE_061")]
internal static class Sim_LOE_061
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            Sim_LOE_061e.Rule.ApplyTo(StatTarget.RandomOtherFriendlyMinion),
        ],
    };
}
