using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 嗜法者
// Spellslinger
// 战吼：双方玩家各获取一张随机法术牌，你获取的那张法力值消耗减少（2）点。
// Battlecry: Both players get a random spell. Yours costs (2) less.
[CardSim("AT_007")]
internal static class Sim_AT_007
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Spell }, CostDelta: -2, EnchantmentCardId: Sim_AT_007e.CardId),
            new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Spell }, Recipient: CardRecipient.Opponent),
        ],
    };
}
