namespace Silverfish.Simulation.Cards;

// 范达尔·鹿盔
// Fandral Staghelm
// 你的抉择牌和英雄技能可以同时拥有两种效果。
// Your Choose One cards and powers have both effects combined.
[CardSim("OG_044")]
internal static class Sim_OG_044
{
    internal static CardRules Rules { get; } = new()
    {
        CombineFriendlyChoices = true,
    };
}
