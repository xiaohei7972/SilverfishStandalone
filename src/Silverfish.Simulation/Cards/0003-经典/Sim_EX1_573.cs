using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 塞纳留斯
// Cenarius
// 抉择：使你的所有其他随从获得+2/+2；或者召唤两个2/2并具有嘲讽的树人。
// Choose One - Give your other minions +2/+2; or Summon two 2/2 Treants with Taunt.
[CardSim("EX1_573")]
internal static class Sim_EX1_573
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("buffMinions", [Sim_EX1_573ae.Rule.ApplyTo(StatTarget.OtherFriendlyMinions)]),
            new("summonTreants",
            [
                new SummonEffect("EX1_573t", SummonPlacement.RightOfSource),
                new SummonEffect("EX1_573t", SummonPlacement.LeftOfSource)
            ])
        ]
    };
}
