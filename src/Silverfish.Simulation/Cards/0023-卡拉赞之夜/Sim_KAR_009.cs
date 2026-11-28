using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 呓语魔典
// Babbling Book
// 战吼：随机将一张法师法术牌置入你的 手牌。
// Battlecry: Add a random Mage spell to your hand.
[CardSim("KAR_009")]
internal static class Sim_KAR_009
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Spell, CardClass = TAG_CLASS.MAGE })],
    };
}
