namespace Silverfish.Simulation.Cards;

[CardSim("EX1_082")]
internal static class Sim_EX1_082
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.OtherCharacters, 1, Count: 3)]
    };
}
