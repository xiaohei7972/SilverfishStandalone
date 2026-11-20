namespace Silverfish.Simulation.Cards;

[CardSim("BT_130")]
internal static class Sim_BT_130
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 2, OverflowCardId: "CS2_013t")]
    };
}
