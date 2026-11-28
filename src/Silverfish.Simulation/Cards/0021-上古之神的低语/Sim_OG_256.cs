using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 恩佐斯的子嗣
// Spawn of N'Zoth
// 亡语：使你的所有随从获得+1/+1。
// Deathrattle: Give your minions +1/+1.
[CardSim("OG_256")]
internal static class Sim_OG_256
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            Sim_OG_256e.Rule.ApplyTo(StatTarget.FriendlyMinions),
        ],
    };
}
