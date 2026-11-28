using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 唤雾者伊戈瓦尔
// The Mistcaller
// 战吼：使你的手牌和牌库里的所有随从牌获得+1/+1。
// Battlecry: Give all minions in your hand and deck +1/+1.
[CardSim("AT_054")]
internal static class Sim_AT_054
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_AT_045e.Rule.ApplyTo(StatTarget.FriendlyHandAndDeck),
        ],
    };
}
