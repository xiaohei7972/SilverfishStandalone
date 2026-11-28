namespace Silverfish.Simulation.Cards;

// 活化铠甲
// Animated Armor
// 你的英雄每次只会受到1点伤害。
// Your hero can only take 1 damage at a time.
[CardSim("LOE_119")]
internal static class Sim_LOE_119
{
    internal static CardRules Rules { get; } = new()
    {
        FriendlyHeroDamageCap = 1,
    };
}
