using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 加固
// Bolster
// 使你具有嘲讽的随从获得+2/+2。
// Give your Taunt minions +2/+2.
[CardSim("AT_068")]
internal static class Sim_AT_068
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_AT_068e.Rule.ApplyTo(StatTarget.FriendlyMinions, new CardFilter { HasTaunt = true }),
        ],
    };
}
