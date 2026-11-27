namespace Silverfish.Simulation.Cards;

// 老式治疗机器人
// Antique Healbot
// 战吼：为你的英雄恢复8点生命值。
// Battlecry: Restore 8 Health to your hero.
[CardSim("GVG_069")]
internal static class Sim_GVG_069
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new HealEffect(HealTarget.FriendlyHero, 8)]
    };
}
