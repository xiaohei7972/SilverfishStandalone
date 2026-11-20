namespace Silverfish.Simulation.Cards;

internal static class Sim_HERO_01bp
{
    internal const string CardId = "HERO_01bp";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new GainArmorEffect(2)]
    };
}
