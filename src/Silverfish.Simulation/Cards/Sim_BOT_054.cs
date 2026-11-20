namespace Silverfish.Simulation.Cards;

[CardSim("BOT_054")]
internal static class Sim_BOT_054
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainFilled, 2, ManaRecipient.BothPlayers)]
    };
}
