using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 黑暗教徒
// Dark Cultist
// 亡语：随机使一个友方随从获得+3生命值。
// Deathrattle: Give a random friendly minion +3 Health.
[CardSim("FP1_023")]
internal static class Sim_FP1_023
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            Sim_FP1_023e.Rule.ApplyTo(StatTarget.RandomOtherFriendlyMinion)
        ]
    };
}
