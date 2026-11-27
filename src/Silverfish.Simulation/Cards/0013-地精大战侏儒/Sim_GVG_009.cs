namespace Silverfish.Simulation.Cards;

// 暗影投弹手
// Shadowbomber
// 战吼：对每个英雄造成3点伤害。
// Battlecry: Deal 3 damage to each hero.
[CardSim("GVG_009")]
internal static class Sim_GVG_009
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.FriendlyHero, 3),
            new DamageEffect(DamageTarget.EnemyHero, 3)
        ]
    };
}
