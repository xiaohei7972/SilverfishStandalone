using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 上古之神的小精灵
// Wisps of the Old Gods
// 抉择：召唤七个1/1的小精灵；或者使你的所有随从获得+2/+2。
// Choose One - Summon seven 1/1 Wisps; or Give your minions +2/+2.
[CardSim("OG_195")]
internal static class Sim_OG_195
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("OG_195a", [new SummonEffect("OG_195c"), new SummonEffect("OG_195c"), new SummonEffect("OG_195c"), new SummonEffect("OG_195c"), new SummonEffect("OG_195c"), new SummonEffect("OG_195c"), new SummonEffect("OG_195c")]) { PlayReqs = [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)] },
            new("OG_195b", [Sim_OG_195e.Rule.ApplyTo(StatTarget.FriendlyMinions)]),
        ],
    };
}
