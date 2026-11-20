namespace Silverfish.Simulation.Cards;

[CardSim("OG_026")]
internal static class Sim_OG_026
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new UnlockOverloadEffect()]
    };
}
