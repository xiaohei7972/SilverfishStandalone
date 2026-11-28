using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 子弹上膛
// Lock and Load
// 在本回合中，每当你施放一个法术，随机获取一张 猎人卡牌。
// Each time you cast a spell this turn, get a random Hunter card.
[CardSim("AT_061")]
internal static class Sim_AT_061
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RewardSpellCastsThisTurnEffect(TAG_CLASS.HUNTER, Sim_AT_061e.CardId),
        ],
    };
}
