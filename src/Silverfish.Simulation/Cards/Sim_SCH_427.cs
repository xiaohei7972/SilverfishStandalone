namespace Silverfish.Simulation.Cards;

[CardSim("SCH_427")]
internal static class Sim_SCH_427
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.Refresh, 2)]
    };
}
