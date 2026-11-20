namespace Silverfish.Simulation.Cards;

internal static class Sim_BOT_054
{
    internal const string CardId = "BOT_054";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ChangeManaEffect(ManaOperation.GainFilled, 2, ManaRecipient.BothPlayers)]
    };
}
