using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 黑暗鸦人
// Dark Arakkoa
// 嘲讽，战吼：使你的克苏恩获得+4/+4（无论它在哪里）。
// Taunt Battlecry: Give your C'Thun +4/+4 (wherever it is).
[CardSim("OG_293")]
internal static class Sim_OG_293
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EmpowerCardEffect("OG_280", Sim_OG_293f.Rule.Modifier, Sim_OG_293f.Rule.CardId),
        ],
    };
}
