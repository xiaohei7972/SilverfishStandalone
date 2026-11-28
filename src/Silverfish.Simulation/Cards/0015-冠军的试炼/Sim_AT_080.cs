namespace Silverfish.Simulation.Cards;

// 要塞指挥官
// Garrison Commander
// 每个回合你可以使用两次英雄技能。
// You can use your Hero Power twice a turn.
[CardSim("AT_080")]
internal static class Sim_AT_080
{
    internal static CardRules Rules { get; } = new()
    {
        HeroPowerUsesAllowed = 2,
    };
}
