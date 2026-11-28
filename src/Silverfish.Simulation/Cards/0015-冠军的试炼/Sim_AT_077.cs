using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 白银之枪
// Argent Lance
// 战吼：揭示双方牌库里的一张随从牌。如果你的牌法力值消耗较大，+1耐久度。
// Battlecry: Reveal a minion in each deck. If yours costs more, +1 Durability.
[CardSim("AT_077")]
internal static class Sim_AT_077
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new JoustEffect([Sim_AT_077e.Rule.ApplyToWeapon() with { UseSource = true }]),
        ],
    };
}
