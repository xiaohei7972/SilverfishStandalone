namespace Silverfish.Simulation.Cards;

// 腐化治疗机器人
// Corrupted Healbot
// 亡语：为敌方英雄恢复#8点生命值。
// Deathrattle: Restore #8 Health to the enemy hero.
[CardSim("OG_147")]
internal static class Sim_OG_147
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new HealEffect(HealTarget.EnemyHero, 8),
        ],
    };
}
