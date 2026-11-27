using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 玛尔加尼斯
// Mal'Ganis
// 你的英雄免疫。你的其他恶魔拥有+2/+2。
// Your hero is Immune. Your other Demons have +2/+2.
[CardSim("GVG_021")]
internal static class Sim_GVG_021
{
    internal static CardRules Rules { get; } = new()
    {
        FriendlyHeroImmune = true,
        Auras =
        [
            new StatAura(AuraRecipients.OtherFriendlyMinions, new(StatModifierMode.Add, 2, 2))
            {
                CardFilter = new CardFilter { MinionType = MinionType.Demon },
                EnchantmentCardId = Sim_GVG_021e.CardId
            }
        ]
    };
}
