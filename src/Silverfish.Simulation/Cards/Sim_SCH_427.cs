namespace Silverfish.Simulation.Cards;

internal static class Sim_SCH_427
{
    internal const string CardId = "SCH_427";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.Refresh, 2)]
    };
}
