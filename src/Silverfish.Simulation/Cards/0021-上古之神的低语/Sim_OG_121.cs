using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 古加尔
// Cho'gall
// 战吼：将你在本局对战中弃掉的所有牌移回你的手牌，这些牌会消耗生命值而非法力值。
// Battlecry: Return all cards you discarded this game to your hand. They cost Health instead of Mana.
[CardSim("OG_121")]
internal static class Sim_OG_121
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ReturnDiscardedCardsEffect(Sim_OG_121e.CardId),
        ],
    };
}
