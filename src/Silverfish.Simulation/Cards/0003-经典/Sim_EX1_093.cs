using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 阿古斯防御者
// Defender of Argus
// 战吼：使相邻的随从获得+1/+1和嘲讽。
// Battlecry: Give adjacent minions +1/+1 and Taunt.
[CardSim("EX1_093")]
internal static class Sim_EX1_093
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_093e.Rule.ApplyTo(StatTarget.AdjacentFriendlyMinions)]
    };
}
