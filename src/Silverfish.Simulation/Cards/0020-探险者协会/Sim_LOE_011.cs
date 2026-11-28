namespace Silverfish.Simulation.Cards;

// 雷诺·杰克逊
// Reno Jackson
// 战吼：如果你的牌库里没有相同的牌，则为你的英雄恢复所有生命值。
// Battlecry: If your deck has no duplicates, fully heal your hero.
[CardSim("LOE_011")]
internal static class Sim_LOE_011
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new NoDuplicatesInDeckCondition(), [new HealEffect(HealTarget.FriendlyHero, 999)]),
        ],
    };
}
