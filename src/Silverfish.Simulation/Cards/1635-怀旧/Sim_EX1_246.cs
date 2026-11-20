namespace Silverfish.Simulation.Cards;

// 妖术
// Hex
// 使一个随从变形成为一只0/1并具有嘲讽的青蛙。
// Transform a minion into a 0/1 Frog with Taunt.
[CardSim("EX1_246")]
internal static class Sim_EX1_246
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [new TransformMinionEffect("hexfrog")]
    };
}
