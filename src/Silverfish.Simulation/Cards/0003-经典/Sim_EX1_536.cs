using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 鹰角弓
// Eaglehorn Bow
// 每当一个友方奥秘被揭示时，便获得+1耐久度。
// Whenever a friendly Secret is revealed, gain +1 Durability.
[CardSim("EX1_536")]
internal static class Sim_EX1_536
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.SecretRevealed, TriggerRelation.Friendly, [new ModifyWeaponEffect(DurabilityDelta: 1, EnchantmentCardId: Sim_EX1_536e.CardId)])]
    };
}
