namespace Silverfish.Simulation.Cards;

internal static class Sim_LOOT_392
{
    internal const string CardId = "LOOT_392";

    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new ChangeManaEffect(ManaOperation.Refresh, int.MaxValue)]
    };
}
