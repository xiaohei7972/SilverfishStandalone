namespace Silverfish.Simulation.Cards;

// 真银圣剑
// Truesilver Champion
// 每当你的英雄进攻，便为其恢复#3点生命值。
// Whenever your hero attacks, restore #3_Health to it.
[CardSim("CS2_097")]
internal static class Sim_CS2_097
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.HeroAttacking, TriggerRelation.Friendly, [new HealFriendlyHeroEffect(3)])]
    };
}
