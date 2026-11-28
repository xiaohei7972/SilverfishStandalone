namespace Silverfish.Simulation.Cards;

// 宴会牧师
// Priest of the Feast
// 每当你施放一个法术，为你的英雄恢复#3点生命值。
// Whenever you cast a spell, restore #3 Health to your hero.
[CardSim("KAR_035")]
internal static class Sim_KAR_035
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [new HealFriendlyHeroEffect(3)]),
        ],
    };
}
