namespace Silverfish.Simulation.Cards;

// 浸毒匕首
// Poisoned Daggers
// 英雄技能 装备一把2/2的 匕首。
// Hero Power Equip a 2/2 Weapon.
[CardSim("HERO_03bp2")]
internal static class Sim_HERO_03bp2
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EquipWeaponEffect("AT_132_ROGUEt"),
        ],
    };
}
