namespace Silverfish.Simulation.Cards;

// 小鬼首领
// Imp Gang Boss
// 每当本随从受到伤害，召唤一个1/1的小鬼。
// Whenever this minion takes damage, summon a 1/1 Imp.
[CardSim("BRM_006")]
internal static class Sim_BRM_006
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.Damage,
                TriggerRelation.Friendly,
                [new SummonEffect("BRM_006t")])
            {
                EntityMatch = TriggerEntityMatch.Self
            }
        ]
    };
}
