namespace Silverfish.Simulation.Cards;

// 烈焰小鬼
// Flame Imp
// 战吼：&对你的英雄造成3点伤害。
// Battlecry:& Deal 3 damage to your hero.
[CardSim("EX1_319")]
internal static class Sim_EX1_319
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.FriendlyHero, 3)]
    };
}
