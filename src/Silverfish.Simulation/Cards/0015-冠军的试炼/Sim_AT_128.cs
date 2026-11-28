namespace Silverfish.Simulation.Cards;

// 骷髅骑士
// The Skeleton Knight
// 亡语：揭示双方牌库里的一张随从牌。如果你的牌法力值消耗较大，则将骷髅骑士移回你的手牌。
// Deathrattle: Reveal a minion in each deck. If yours costs more, return this to your hand.
[CardSim("AT_128")]
internal static class Sim_AT_128
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new JoustEffect([new ReturnSourceCardEffect()]),
        ],
    };
}
