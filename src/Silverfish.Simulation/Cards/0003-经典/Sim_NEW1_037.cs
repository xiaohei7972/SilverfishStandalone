using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 铸剑师
// Master Swordsmith
// 在你的回合结束时，随机使另一个友方随从获得+1攻击力。
// At the end of your turn, give another random friendly minion +1 Attack.
[CardSim("NEW1_037")]
internal static class Sim_NEW1_037
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [Sim_NEW1_037e.Rule.ApplyTo(StatTarget.RandomOtherFriendlyMinion)])]
    };
}
