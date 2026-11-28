namespace Silverfish.Simulation.Cards;

// 恐怖的奴隶主
// Grim Patron
// 在本随从受到伤害并存活下来后，召唤另一个恐怖的奴隶主。
// After this minion survives damage, summon another Grim Patron.
[CardSim("BRM_019")]
internal static class Sim_BRM_019
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.Damage,
                TriggerRelation.Friendly,
                [new SummonEffect("BRM_019")])
            {
                EntityMatch = TriggerEntityMatch.Self,
                RequireLivingEntity = true
            }
        ]
    };
}
