namespace Silverfish.Simulation.Cards;

internal static class Sim_NEW1_031
{
    internal const string CardId = "NEW1_031";

    internal static CardRules Rules { get; } = new()
    {
        Requirements = PlayRequirements.BoardSpace,
        OnPlay = [new SummonRandomMinionEffect(["NEW1_032", "NEW1_033", "NEW1_034"])]
    };
}
