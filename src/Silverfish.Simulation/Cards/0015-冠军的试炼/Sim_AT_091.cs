namespace Silverfish.Simulation.Cards;

// 赛场医师
// Tournament Medic
// 激励：为你的英雄恢复#2点生命值。
// Inspire: Restore #2 Health to your hero.
[CardSim("AT_091")]
internal static class Sim_AT_091
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new HealEffect(HealTarget.FriendlyHero, 2)]),
        ],
    };
}
