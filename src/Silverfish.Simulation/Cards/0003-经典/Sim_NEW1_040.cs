namespace Silverfish.Simulation.Cards;

// 霍格
// Hogger
// 在你的回合结束时，召唤一个2/2并具有嘲讽的豺狼人。
// At the end of your turn, summon a 2/2 Gnoll with_Taunt.
[CardSim("NEW1_040")]
internal static class Sim_NEW1_040
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new SummonEffect("NEW1_040t")])]
    };
}
