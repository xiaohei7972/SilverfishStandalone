using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 集结之刃
// Rallying Blade
// 战吼：使你具有圣盾的随从获得+1/+1。
// Battlecry: Give +1/+1 to your minions with Divine Shield.
[CardSim("OG_222")]
internal static class Sim_OG_222
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_OG_222e.Rule.ApplyTo(StatTarget.FriendlyMinions) with { ShieldedOnly = true },
        ],
    };
}
