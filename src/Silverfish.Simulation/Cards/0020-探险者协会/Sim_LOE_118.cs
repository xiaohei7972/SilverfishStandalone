namespace Silverfish.Simulation.Cards;

// 诅咒之刃
// Cursed Blade
// 你的英雄受到的所有伤害效果翻倍。
// Double all damage dealt to your hero.
[CardSim("LOE_118")]
internal static class Sim_LOE_118
{
    internal static CardRules Rules { get; } = new()
    {
        FriendlyHeroDamageMultiplier = 2,
    };
}
