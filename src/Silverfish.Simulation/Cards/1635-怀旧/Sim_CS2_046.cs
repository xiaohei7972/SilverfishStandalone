using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 嗜血
// Bloodlust
// 在本回合中，使你的所有随从获得+3攻击力。
// Give your minions +3_Attack this turn.
[CardSim("CS2_046")]
internal static class Sim_CS2_046
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_CS2_046e.Rule.ApplyTo(StatTarget.FriendlyMinions)]
    };
}
