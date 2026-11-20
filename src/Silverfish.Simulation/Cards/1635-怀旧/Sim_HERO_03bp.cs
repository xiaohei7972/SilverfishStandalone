namespace Silverfish.Simulation.Cards;

// 匕首精通
// Dagger Mastery
// 英雄技能 装备一把1/2的 匕首。
// Hero Power Equip a 1/2 Dagger.
[CardSim("HERO_03bp")]
internal static class Sim_HERO_03bp
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new EquipWeaponEffect("CS2_082")]
    };
}
