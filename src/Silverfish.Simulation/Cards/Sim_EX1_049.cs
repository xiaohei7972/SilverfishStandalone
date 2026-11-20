namespace Silverfish.Simulation.Cards;

[CardSim("EX1_049")]
internal static class Sim_EX1_049
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.FriendlyMinion,
        OnPlay = [new ReturnMinionEffect()]
    };
}
