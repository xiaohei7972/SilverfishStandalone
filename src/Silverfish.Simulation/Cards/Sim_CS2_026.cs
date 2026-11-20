namespace Silverfish.Simulation.Cards;

internal static class Sim_CS2_026
{
    internal const string CardId = "CS2_026";

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new FreezeEffect(FreezeTarget.EnemyMinions)]
    };
}
