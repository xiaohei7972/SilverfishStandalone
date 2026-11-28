using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 野性之力
// Power of the Wild
// 抉择：使你的所有随从获得+1/+1；或者召唤一只3/2的 猎豹。
// Choose One - Give your minions +1/+1; or Summon a 3/2 Panther.
[CardSim("EX1_160")]
internal static class Sim_EX1_160
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("EX1_160b", [Sim_EX1_160be.Rule.ApplyTo(StatTarget.FriendlyMinions)]),
            new("EX1_160a", [new SummonEffect("EX1_160t")])
            {
                PlayReqs = [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)]
            }
        ]
    };
}
