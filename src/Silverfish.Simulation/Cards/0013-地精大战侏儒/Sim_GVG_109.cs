namespace Silverfish.Simulation.Cards;

// 小个子法师
// Mini-Mage
// 潜行，法术伤害+1
// Stealth Spell Damage +1
[CardSim("GVG_109")]
internal static class Sim_GVG_109
{
    internal static CardRules Rules { get; } = new()
    {
        SpellDamage = 1
    };
}
