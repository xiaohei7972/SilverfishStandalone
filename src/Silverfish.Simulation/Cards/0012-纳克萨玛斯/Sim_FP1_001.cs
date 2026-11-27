using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 肉用僵尸
// Zombie Chow
// 亡语：为敌方英雄恢复5点生命值。
// Deathrattle: Restore 5 Health to the enemy hero.
[CardSim("FP1_001")]
internal static class Sim_FP1_001
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new HealEffect(HealTarget.EnemyHero, 5)
        ]
    };
}
