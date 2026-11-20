namespace Silverfish.Simulation.Cards;

[CardSim("EX1_169")]
internal static class Sim_EX1_169
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainTemporary, 1)]
    };
}
