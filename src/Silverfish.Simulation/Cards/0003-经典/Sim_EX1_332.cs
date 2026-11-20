namespace Silverfish.Simulation.Cards;

// 沉默
// Silence
// 沉默一个随从。
// Silence a minion.
[CardSim("EX1_332")]
internal static class Sim_EX1_332
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [new SilenceEffect()]
    };
}
