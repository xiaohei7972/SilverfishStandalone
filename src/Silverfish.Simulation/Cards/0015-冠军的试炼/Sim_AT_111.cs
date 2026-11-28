namespace Silverfish.Simulation.Cards;

// 零食商贩
// Refreshment Vendor
// 战吼：为每个英雄恢复#4点生命值。
// Battlecry: Restore #4 Health to each hero.
[CardSim("AT_111")]
internal static class Sim_AT_111
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new HealEffect(HealTarget.FriendlyHero, 4),
            new HealEffect(HealTarget.EnemyHero, 4),
        ],
    };
}
