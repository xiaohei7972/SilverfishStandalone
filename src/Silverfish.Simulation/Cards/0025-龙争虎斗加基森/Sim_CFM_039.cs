namespace Silverfish.Simulation.Cards;

// 杂耍小鬼
// Street Trickster
// 法术伤害+2
// Spell Damage +2
[CardSim("CFM_039")]
internal static class Sim_CFM_039
{
    internal static CardRules Rules { get; } = new() { SpellDamage = 2 };
}
