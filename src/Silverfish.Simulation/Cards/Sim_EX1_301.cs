namespace Silverfish.Simulation.Cards;

[CardSim("EX1_301")]
internal static class Sim_EX1_301
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.Destroy, 1)]
    };
}
