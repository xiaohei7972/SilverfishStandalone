namespace Silverfish.Simulation.Cards;

[CardSim("LOOT_351")]
internal static class Sim_LOOT_351
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new ChangeManaEffect(ManaOperation.GainEmpty, 1)]
    };
}
