namespace Silverfish.Simulation.Cards;

// 玛里苟斯
// Malygos
// 法术伤害+5
// Spell Damage +5
[CardSim("EX1_563")]
internal static class Sim_EX1_563
{
    internal static CardRules Rules { get; } = new()
    {
        SpellDamage = 5
    };
}
