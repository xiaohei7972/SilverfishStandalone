namespace Silverfish.Simulation.Cards;

// 皇家雷象
// King's Elekk
// 战吼：揭示双方牌库里的一张随从牌。如果你的牌法力值消耗较大，抽这张牌。
// Battlecry: Reveal a minion in each deck. If yours costs more, draw it.
[CardSim("AT_058")]
internal static class Sim_AT_058
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new JoustEffect([], DrawRevealedMinion: true),
        ],
    };
}
