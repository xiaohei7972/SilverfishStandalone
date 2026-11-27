using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 小个子扰咒师
// Wee Spellstopper
// 相邻的随从拥有扰魔。
// Adjacent minions have Elusive.
[CardSim("GVG_122")]
internal static class Sim_GVG_122
{
    internal static CardRules Rules { get; } = new()
    {
        Auras =
        [
            new StatAura(AuraRecipients.AdjacentFriendlyMinions)
            {
                GrantedTag = GAME_TAG.ELUSIVE,
                EnchantmentCardId = Sim_GVG_122e.CardId
            }
        ]
    };
}
