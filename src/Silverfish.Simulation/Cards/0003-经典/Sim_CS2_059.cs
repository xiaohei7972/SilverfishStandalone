using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 鲜血小鬼
// Blood Imp
// 潜行 在你的回合结束时，随机使另一个友方随从获得+1生命值。
// Stealth. At the end of your turn, give another random friendly minion +1 Health.
[CardSim("CS2_059")]
internal static class Sim_CS2_059
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [Sim_CS2_059o.Rule.ApplyTo(StatTarget.RandomOtherFriendlyMinion)])]
    };
}
