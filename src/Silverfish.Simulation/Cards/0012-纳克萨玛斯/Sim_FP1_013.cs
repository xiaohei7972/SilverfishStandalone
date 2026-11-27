namespace Silverfish.Simulation.Cards;

// 克尔苏加德
// Kel'Thuzad
// 在每个回合结束时，召唤所有在本回合中死亡的友方随从。
// At the end of each turn, summon all friendly minions that died this turn.
[CardSim("FP1_013")]
internal static class Sim_FP1_013
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnEnded,
                TriggerRelation.Any,
                [new ResurrectDiedThisTurnEffect()])
        ]
    };
}
