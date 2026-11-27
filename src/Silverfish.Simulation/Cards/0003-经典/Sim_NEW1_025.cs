using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 血帆海盗
// Bloodsail Corsair
// 战吼：使对手的武器失去1点耐久度。
// Battlecry: Remove 1 Durability from your opponent's weapon.
[CardSim("NEW1_025")]
internal static class Sim_NEW1_025
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ModifyWeaponEffect(DurabilityDelta: -1, TargetWeapon: ConditionPlayer.Opponent, EnchantmentCardId: Sim_NEW1_025e.CardId)]
    };
}
