using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 斯克拉姆狂热者
// Skeram Cultist
// 战吼： 使你的克苏恩 获得+2/+2（无论它在哪里）。
// Battlecry: Give your C'Thun +2/+2 (wherever it is).
[CardSim("OG_339")]
internal static class Sim_OG_339
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EmpowerCardEffect("OG_280", Sim_OG_339e.Rule.Modifier, Sim_OG_339e.Rule.CardId),
        ],
    };
}
