using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 邪灵召唤师
// Beckoner of Evil
// 战吼：使你的克苏恩获得+2/+2（无论它在哪里）。
// Battlecry: Give your C'Thun +2/+2 (wherever it is).
[CardSim("OG_281")]
internal static class Sim_OG_281
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EmpowerCardEffect("OG_280", Sim_OG_281e.Rule.Modifier, Sim_OG_281e.Rule.CardId),
        ],
    };
}
