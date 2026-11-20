namespace Silverfish.Simulation.Cards;

[CardSim("FP1_021")]
internal static class Sim_FP1_021
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new DamageEffect(DamageTarget.AllMinions, 1)]
    };
}
