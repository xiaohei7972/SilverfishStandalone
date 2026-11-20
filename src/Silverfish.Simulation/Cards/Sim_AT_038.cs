namespace Silverfish.Simulation.Cards;

[CardSim("AT_038")]
internal static class Sim_AT_038
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 1)],
        Deathrattle = [new ChangeManaEffect(ManaOperation.Destroy, 1)]
    };
}
