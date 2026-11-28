namespace Silverfish.Simulation.Cards;

// 冰冷的雪地狗头人
// Frigid Snobold
// 法术伤害+1
// Spell Damage +1
[CardSim("AT_093")]
internal static class Sim_AT_093
{
    internal static CardRules Rules { get; } = new()
    {
        SpellDamage = 1,
    };
}
