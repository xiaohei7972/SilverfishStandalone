using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 重甲战马
// Armored Warhorse
// 战吼：揭示双方牌库里的一张随从牌。如果你的牌法力值消耗较大，则获得冲锋。
// Battlecry: Reveal a minion in each deck. If yours costs more, gain Charge.
[CardSim("AT_108")]
internal static class Sim_AT_108
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new JoustEffect([new EnchantStatsEffect(StatTarget.Source) { GrantedTag = GAME_TAG.CHARGE }]),
        ],
    };
}
