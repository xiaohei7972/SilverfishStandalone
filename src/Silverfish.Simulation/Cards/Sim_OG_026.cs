namespace Silverfish.Simulation.Cards;

internal static class Sim_OG_026
{
    internal const string CardId = "OG_026";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new UnlockOverloadEffect()]
    };
}
