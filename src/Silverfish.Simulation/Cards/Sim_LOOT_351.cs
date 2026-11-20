namespace Silverfish.Simulation.Cards;

internal static class Sim_LOOT_351
{
    internal const string CardId = "LOOT_351";

    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new ChangeManaEffect(ManaOperation.GainEmpty, 1)]
    };
}
