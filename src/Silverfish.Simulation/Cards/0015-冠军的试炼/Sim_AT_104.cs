namespace Silverfish.Simulation.Cards;

// 海象人龟骑士
// Tuskarr Jouster
// 战吼：揭示双方牌库里的一张随从牌。如果你的牌法力值消耗较大，则为你的英雄恢复#7点生命值。
// Battlecry: Reveal a minion in each deck. If yours costs more, restore #7 Health to your hero.
[CardSim("AT_104")]
internal static class Sim_AT_104
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new JoustEffect([new HealEffect(HealTarget.FriendlyHero, 7)]),
        ],
    };
}
