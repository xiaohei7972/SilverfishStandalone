using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 厄运召唤者
// Doomcaller
// 战吼：使你的克苏恩获得+2/+2（无论它在哪里）。如果克苏恩死亡，将其洗入你的牌库。
// Battlecry: Give your C'Thun +2/+2 (wherever it is). If it's dead, shuffle it into your deck.
[CardSim("OG_255")]
internal static class Sim_OG_255
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EmpowerCardEffect("OG_280", Sim_OG_281e.Rule.Modifier, Sim_OG_281e.Rule.CardId) { ReshuffleIfDead = true },
        ],
    };
}
