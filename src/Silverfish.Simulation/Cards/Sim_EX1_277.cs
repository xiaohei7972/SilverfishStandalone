namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_277
{
    internal const string CardId = "EX1_277";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.EnemyCharacters, 1, Count: 3)]
    };
}
