namespace Silverfish.Simulation.Cards;

internal static class Sim_NEW1_019
{
    internal const string CardId = "NEW1_019";

    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly,
            [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1)])]
    };
}
