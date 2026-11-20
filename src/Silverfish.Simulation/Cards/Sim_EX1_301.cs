namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_301
{
    internal const string CardId = "EX1_301";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.Destroy, 1)]
    };
}
