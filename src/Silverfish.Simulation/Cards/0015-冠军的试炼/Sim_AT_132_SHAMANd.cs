namespace Silverfish.Simulation.Cards;

// 空气之怒图腾
// Wrath of Air Totem
// 法术伤害+1
// Spell Damage +1
[CardSim("AT_132_SHAMANd")]
internal static class Sim_AT_132_SHAMANd
{
    internal static CardRules Rules { get; } = new()
    {
        SpellDamage = 1,
    };
}
