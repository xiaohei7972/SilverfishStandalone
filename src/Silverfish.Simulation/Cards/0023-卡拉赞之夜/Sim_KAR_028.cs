namespace Silverfish.Simulation.Cards;

// 愚者之灾
// Fool's Bane
// 每个回合攻击次数不限，但无法攻击英雄。
// Unlimited attacks each turn. Can't attack heroes.
[CardSim("KAR_028")]
internal static class Sim_KAR_028
{
    internal static CardRules Rules { get; } = new()
    {
        UnlimitedHeroAttacks = true,
        CannotAttackHeroes = true,
    };
}
