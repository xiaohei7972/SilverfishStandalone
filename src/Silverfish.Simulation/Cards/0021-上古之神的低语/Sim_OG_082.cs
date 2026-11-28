namespace Silverfish.Simulation.Cards;

// 异变的狗头人
// Evolved Kobold
// 法术伤害+2
// Spell Damage +2
[CardSim("OG_082")]
internal static class Sim_OG_082
{
    internal static CardRules Rules { get; } = new()
    {
        SpellDamage = 2,
    };
}
