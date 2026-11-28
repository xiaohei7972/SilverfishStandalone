namespace Silverfish.Simulation.Cards;

// 幽灵之爪
// Spirit Claws
// 当你拥有法术伤害时，拥有 +2攻击力。
// Has +2 Attack while you have Spell Damage.
[CardSim("KAR_063")]
internal static class Sim_KAR_063
{
    internal static CardRules Rules { get; } = new()
    {
        WeaponAuras = [new WeaponAttackAura(2, AuraCondition.WhileHavingSpellDamage)],
    };
}
