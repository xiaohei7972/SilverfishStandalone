using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 血帆袭击者
// Bloodsail Raider
// 战吼： 获得等同于你的武器攻击力的攻击力。
// Battlecry: Gain Attack equal to the Attack of your weapon.
[CardSim("NEW1_018")]
internal static class Sim_NEW1_018
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ScaledEnchantStatsEffect(StatTarget.Source, StatScaleSource.EquippedWeaponAttack, AttackPerUnit: 1, EnchantmentCardId: Sim_NEW1_018e.CardId)]
    };
}
