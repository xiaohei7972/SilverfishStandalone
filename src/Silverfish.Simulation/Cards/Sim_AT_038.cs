namespace Silverfish.Simulation.Cards;

internal static class Sim_AT_038
{
    internal const string CardId = "AT_038";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainEmpty, 1)],
        Deathrattle = [new ChangeManaEffect(ManaOperation.Destroy, 1)]
    };
}
