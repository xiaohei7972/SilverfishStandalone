namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_097
{
    internal const string CardId = "CS2_097";

    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.HeroAttacking, TriggerRelation.Friendly, [new HealFriendlyHeroEffect(3)])]
    };
}
