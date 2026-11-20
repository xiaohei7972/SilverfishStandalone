namespace Silverfish.Simulation.Cards;

// 变形术
// Polymorph
// 使一个随从变形成为1/1的绵羊。
// Transform a minion into a 1/1 Sheep.
[CardSim("CS2_022")]
internal static class Sim_CS2_022
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [new TransformMinionEffect("CS2_tk1")]
    };
}
