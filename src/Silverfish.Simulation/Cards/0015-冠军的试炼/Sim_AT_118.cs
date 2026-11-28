using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 十字军统领
// Grand Crusader
// 战吼： 随机将一张圣骑士牌置入你的手牌。
// Battlecry: Add a random Paladin card to your hand.
[CardSim("AT_118")]
internal static class Sim_AT_118
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateRandomMatchingCardEffect(new CardFilter { CardClass = TAG_CLASS.PALADIN }),
        ],
    };
}
