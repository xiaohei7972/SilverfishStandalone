namespace Silverfish.Simulation.Cards;

// 暗影形态
// Shadowform
// 你的英雄技能变为“造成2点伤害”。
// Your Hero Power becomes 'Deal 2 damage.'
[CardSim("EX1_625")]
internal static class Sim_EX1_625
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ReplaceHeroPowerEffect("EX1_625t")]
    };
}
