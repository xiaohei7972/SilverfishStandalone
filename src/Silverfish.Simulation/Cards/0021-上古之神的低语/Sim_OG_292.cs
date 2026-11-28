using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 狼人追猎者
// Forlorn Stalker
// 战吼：使你手牌中所有亡语随从牌获得+1/+1。
// Battlecry: Give all Deathrattle minions in your hand +1/+1.
[CardSim("OG_292")]
internal static class Sim_OG_292
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_OG_292e.Rule.ApplyTo(StatTarget.FriendlyHandAndDeck, new CardFilter { HasDeathrattle = true }) with { Target = StatTarget.FriendlyHand },
        ],
    };
}
