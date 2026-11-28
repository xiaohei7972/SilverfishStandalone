namespace Silverfish.Simulation.Cards;

// 考达拉幼龙
// Coldarra Drake
// 你可以使用任意次数的英雄技能。
// You can use your Hero Power any number of times.
[CardSim("AT_008")]
internal static class Sim_AT_008
{
    internal static CardRules Rules { get; } = new()
    {
        HeroPowerUsesAllowed = int.MaxValue,
    };
}
