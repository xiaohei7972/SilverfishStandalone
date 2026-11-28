using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 黄金猿
// Golden Monkey
// 嘲讽 战吼：将你的手牌和牌库里的卡牌替换成传说随从。
// Taunt Battlecry: Replace your hand and deck with Legendary minions.
[CardSim("LOE_019t2")]
internal static class Sim_LOE_019t2
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ReplaceHandAndDeckEffect(new CardFilter { Kind = EntityKind.Minion, IsLegendary = true }),
        ],
    };
}
