namespace Silverfish.Simulation.Cards;

internal static class Sim_CFM_308
{
    internal const string CardId = "CFM_308";

    internal static CardRules Rules { get; } = new()
    {
        Choices = [new("armor", [new GainArmorEffect(10)]), new("refresh", [new ChangeManaEffect(ManaOperation.Refresh, int.MaxValue)])]
    };
}
