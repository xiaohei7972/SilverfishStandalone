using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 丛林之魂
// Soul of the Forest
// 使你的所有随从获得“亡语：召唤一个2/2的树人”。
// Give your minions "Deathrattle: Summon a 2/2 Treant."
[CardSim("EX1_158")]
internal static class Sim_EX1_158
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_158e.Rule.ApplyTo(StatTarget.FriendlyMinions)]
    };
}
