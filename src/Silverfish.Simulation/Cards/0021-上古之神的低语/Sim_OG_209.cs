namespace Silverfish.Simulation.Cards;

// 升腾者海纳泽尔
// Hallazeal the Ascended
// 法术伤害+1 你的法术拥有吸血。
// Spell Damage +1 Your spells have Lifesteal.
[CardSim("OG_209")]
internal static class Sim_OG_209
{
    internal static CardRules Rules { get; } = new()
    {
        SpellDamage = 1,
        SpellsHaveLifesteal = true,
    };
}
