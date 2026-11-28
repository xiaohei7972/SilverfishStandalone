using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 克苏恩的侍从
// C'Thun's Chosen
// 圣盾，战吼：使你的克苏恩获得+3/+3（无论它在哪里）。
// Divine Shield Battlecry: Give your C'Thun +3/+3 (wherever it is).
[CardSim("OG_283")]
internal static class Sim_OG_283
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EmpowerCardEffect("OG_280", new StatModifier(StatModifierMode.Add, 3, 3), Sim_OG_281e.Rule.CardId),
        ],
    };
}
