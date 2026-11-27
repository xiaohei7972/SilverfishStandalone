using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 加兹鲁维
// Gazlowe
// 每当你施放一个法力值消耗为（1）的法术，随机将一张机械牌置入你的手牌。
// Whenever you cast a 1-Cost spell, add a random Mech to your hand.
[CardSim("GVG_117")]
internal static class Sim_GVG_117
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.BeforeSpellResolved,
                TriggerRelation.Friendly,
                [new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Minion, MinionType = MinionType.Mech })])
            {
                CardFilter = new CardFilter { MinimumPrintedCost = 1, MaximumPrintedCost = 1 }
            }
        ]
    };
}
