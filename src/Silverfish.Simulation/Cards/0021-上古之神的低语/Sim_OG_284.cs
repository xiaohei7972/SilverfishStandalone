using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暮光地卜师
// Twilight Geomancer
// 嘲讽。战吼：使你的克苏恩获得+1/+1和嘲讽（无论它在哪里）。
// Taunt Battlecry: Give your C'Thun +1/+1 and Taunt (wherever it is).
[CardSim("OG_284")]
internal static class Sim_OG_284
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EmpowerCardEffect("OG_280", Sim_OG_284e.Rule.Modifier, Sim_OG_284e.Rule.CardId) { GrantedTag = GAME_TAG.TAUNT },
        ],
    };
}
