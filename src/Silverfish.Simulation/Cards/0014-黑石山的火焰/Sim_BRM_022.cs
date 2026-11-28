namespace Silverfish.Simulation.Cards;

// 龙蛋
// Dragon Egg
// 每当本随从受到伤害，召唤一条2/1的雏龙。
// Whenever this minion takes damage, summon a 2/1 Whelp.
[CardSim("BRM_022")]
internal static class Sim_BRM_022
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.Damage,
                TriggerRelation.Friendly,
                [new SummonEffect("BRM_022t")])
            {
                EntityMatch = TriggerEntityMatch.Self
            }
        ]
    };
}
