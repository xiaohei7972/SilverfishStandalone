namespace Silverfish.Simulation.Cards;

internal static class Sim_EX1_169
{
    internal const string CardId = "EX1_169";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainTemporary, 1)]
    };
}
