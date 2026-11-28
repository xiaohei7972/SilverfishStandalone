using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 狂热的新兵
// Zealous Initiate
// 亡语：随机使一个友方随从获得+1/+1。
// Deathrattle: Give a random friendly minion +1/+1.
[CardSim("OG_158")]
internal static class Sim_OG_158
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            Sim_OG_158e.Rule.ApplyTo(StatTarget.RandomOtherFriendlyMinion),
        ],
    };
}
