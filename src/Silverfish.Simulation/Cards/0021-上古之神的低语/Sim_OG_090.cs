using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 秘法宝典
// Cabalist's Tome
// 随机获取3张法师法术牌。
// Get 3 random Mage spells.
[CardSim("OG_090")]
internal static class Sim_OG_090
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Spell, CardClass = TAG_CLASS.MAGE }, 3),
        ],
    };
}
