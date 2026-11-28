using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 饥饿的巨龙
// Hungry Dragon
// 战吼：为你的对手随机召唤一个法力值消耗为（1）的随从。
// Battlecry: Summon a random 1-Cost minion for your opponent.
[CardSim("BRM_026")]
internal static class Sim_BRM_026
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonMatchingMinionEffect(
                new()
                {
                    Kind = EntityKind.Minion,
                    MinimumPrintedCost = 1,
                    MaximumPrintedCost = 1
                },
                1,
                CardRecipient.Opponent)
        ]
    };
}
