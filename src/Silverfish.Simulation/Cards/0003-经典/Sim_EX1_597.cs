namespace Silverfish.Simulation.Cards;

// 小鬼召唤师
// Imp Master
// 在你的回合结束时，对本随从造成1点伤害，并召唤一个1/1的 小鬼。
// At the end of your turn, deal 1 damage to this minion and summon a 1/1 Imp.
[CardSim("EX1_597")]
internal static class Sim_EX1_597
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new DamageEffect(DamageTarget.Source, 1), new SummonEffect("EX1_598")])]
    };
}
