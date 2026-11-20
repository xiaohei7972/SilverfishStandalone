namespace Silverfish.Simulation.Cards;

internal static class Sim_FP1_021
{
    internal const string CardId = "FP1_021";

    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new DamageEffect(DamageTarget.AllMinions, 1)]
    };
}
