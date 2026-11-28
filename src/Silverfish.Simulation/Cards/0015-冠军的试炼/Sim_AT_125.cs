namespace Silverfish.Simulation.Cards;

// 冰吼
// Icehowl
// 冲锋 无法攻击英雄。
// Charge Can't attack heroes.
[CardSim("AT_125")]
internal static class Sim_AT_125
{
    internal static CardRules Rules { get; } = new()
    {
        CannotAttackHeroes = true,
    };
}
