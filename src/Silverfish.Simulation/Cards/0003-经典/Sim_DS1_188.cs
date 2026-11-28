namespace Silverfish.Simulation.Cards;

// 角斗士的长弓
// Gladiator's Longbow
// 你的英雄在攻击时免疫。
// Your hero is Immune while attacking.
[CardSim("DS1_188")]
internal static class Sim_DS1_188
{
    internal static CardRules Rules { get; } = new()
    {
        HeroImmuneWhileAttacking = true
    };
}
