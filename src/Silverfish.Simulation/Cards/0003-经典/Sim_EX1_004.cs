using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 年轻的女祭司
// Young Priestess
// 在你的回合结束时，随机使另一个友方随从获得+1生命值。
// At the end of your turn, give another random friendly minion +1 Health.
[CardSim("EX1_004")]
internal static class Sim_EX1_004
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [Sim_EX1_004e.Rule.ApplyTo(StatTarget.RandomOtherFriendlyMinion)])]
    };
}
