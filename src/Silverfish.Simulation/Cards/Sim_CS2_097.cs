namespace Silverfish.Simulation.Cards;

[CardSim("CS2_097")]
internal static class Sim_CS2_097
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.HeroAttacking, TriggerRelation.Friendly, [new HealFriendlyHeroEffect(3)])]
    };
}
