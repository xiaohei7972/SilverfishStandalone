namespace Silverfish.Simulation.Cards;

[CardSim("HERO_01bp")]
internal static class Sim_HERO_01bp
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new GainArmorEffect(2)]
    };
}
