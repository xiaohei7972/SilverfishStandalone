using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 大师级枪骑士
// Master Jouster
// 战吼：揭示双方牌库里的一张随从牌。如果你的牌法力值消耗较大，则获得嘲讽和圣盾。
// Battlecry: Reveal a minion in each deck. If yours costs more, gain Taunt and Divine Shield.
[CardSim("AT_112")]
internal static class Sim_AT_112
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new JoustEffect([new EnchantStatsEffect(StatTarget.Source) { GrantedTag = GAME_TAG.TAUNT, SecondaryGrantedTag = GAME_TAG.DIVINE_SHIELD }]),
        ],
    };
}
