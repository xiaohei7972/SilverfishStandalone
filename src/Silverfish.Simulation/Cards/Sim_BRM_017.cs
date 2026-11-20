namespace Silverfish.Simulation.Cards;

[CardSim("BRM_017")]
internal static class Sim_BRM_017
{
    internal static CardRules Rules { get; } = new()
    {
        Requirements = PlayRequirements.BoardSpace | PlayRequirements.FriendlyMinionDeath,
        OnPlay = [new ResurrectFriendlyMinionEffect()]
    };
}
