namespace Silverfish.Simulation.Cards;

internal static class Sim_BT_130
{
    internal const string CardId = "BT_130";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 2, OverflowCardId: "CS2_013t")]
    };
}
