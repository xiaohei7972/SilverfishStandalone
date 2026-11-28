namespace Silverfish.Simulation.Cards;

// 热砂港狙击手
// Steamwheedle Sniper
// 你的英雄技能能够以随从为目标。
// Your Hero Power can target minions.
[CardSim("GVG_087")]
internal static class Sim_GVG_087
{
    internal static CardRules Rules { get; } = new()
    {
        AllowHeroPowerTargetMinions = true
    };
}
