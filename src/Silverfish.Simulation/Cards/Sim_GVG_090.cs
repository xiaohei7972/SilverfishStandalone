namespace Silverfish.Simulation.Cards;

[CardSim("GVG_090")]
internal static class Sim_GVG_090
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new RandomDamageEffect(RandomCharacterPool.OtherCharacters, 1, Count: 6)]
    };
}
