using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 黑暗曙光
// A Light in the Darkness
// 发现一张圣骑士随从牌。使其获得+2/+2。
// Discover a Paladin minion. Give it +2/+2.
[CardSim("OG_311")]
internal static class Sim_OG_311
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverCardChoice(new CardFilter { Kind = EntityKind.Minion, CardClass = TAG_CLASS.PALADIN }) { Modifier = Sim_OG_311e.Rule.Modifier, EnchantmentCardId = Sim_OG_311e.Rule.CardId },
    };
}
