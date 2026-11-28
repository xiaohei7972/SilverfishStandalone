namespace Silverfish.Simulation.Cards;

// 扭曲虚空
// Twisting Nether
// 消灭所有随从和地标。
// Destroy all minions and locations.
[CardSim("EX1_312")]
internal static class Sim_EX1_312
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DestroyAllMinionsEffect(IncludeLocations: true)]
    };
}
