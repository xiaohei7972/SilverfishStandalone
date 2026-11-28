using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 寄生感染
// Infest
// 使你的所有随从获得 “亡语：随机将一张野兽牌置入你的手牌”。
// Give your minions "Deathrattle: Add a random Beast to your hand."
[CardSim("OG_045")]
internal static class Sim_OG_045
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_OG_045a.Rule.ApplyTo(StatTarget.FriendlyMinions),
        ],
    };
}
