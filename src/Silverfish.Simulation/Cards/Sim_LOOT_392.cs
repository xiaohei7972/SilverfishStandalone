namespace Silverfish.Simulation.Cards;

[CardSim("LOOT_392")]
internal static class Sim_LOOT_392
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new ChangeManaEffect(ManaOperation.Refresh, int.MaxValue)]
    };
}
