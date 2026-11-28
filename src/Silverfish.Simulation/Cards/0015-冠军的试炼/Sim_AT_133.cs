using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 加基森枪骑士
// Gadgetzan Jouster
// 战吼：揭示双方牌库里的一张随从牌。如果你的牌法力值消耗较大，则获得+1/+1。
// Battlecry: Reveal a minion in each deck. If yours costs more, gain +1/+1.
[CardSim("AT_133")]
internal static class Sim_AT_133
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new JoustEffect([Sim_AT_133e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
