namespace Silverfish.Simulation.Cards;

internal static class Sim_GAME_005
{
    internal const string CardId = "GAME_005";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainTemporary, 1)]
    };
}
