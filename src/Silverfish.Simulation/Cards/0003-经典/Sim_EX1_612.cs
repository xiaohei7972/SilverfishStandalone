using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 肯瑞托法师
// Kirin Tor Mage
// 战吼： 在本回合中，你使用的下一个奥秘的法力值消耗为（0）点。
// Battlecry: The next Secret you play this turn costs (0).
[CardSim("EX1_612")]
internal static class Sim_EX1_612
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new AddTemporaryCostAuraEffect(EntityKind.Spell, CostModifierMode.Set, 0,
                SecretOnly: true, ConsumedOnPlay: true, EnchantmentCardId: Sim_EX1_612o.CardId)
        ]
    };
}
