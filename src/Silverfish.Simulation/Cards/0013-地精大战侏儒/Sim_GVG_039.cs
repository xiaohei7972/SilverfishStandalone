namespace Silverfish.Simulation.Cards;

// 活力图腾
// Vitality Totem
// 在你的回合结束时，为你的英雄恢复4点生命值。
// At the end of your turn, restore 4 Health to your hero.
[CardSim("GVG_039")]
internal static class Sim_GVG_039
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new HealEffect(HealTarget.FriendlyHero, 4)])
        ]
    };
}
