namespace Silverfish.Simulation.Cards;

[CardSim("GAME_005")]
internal static class Sim_GAME_005
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainTemporary, 1)]
    };
}
