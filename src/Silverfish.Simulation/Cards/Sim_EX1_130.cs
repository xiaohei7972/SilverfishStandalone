namespace Silverfish.Simulation.Cards;

[CardSim("EX1_130")]
internal static class Sim_EX1_130
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Friendly, [new SummonAttackDefenderEffect("EX1_130a")])
            {
                Focus = TriggerFocus.AttackDefender, RequiresBoardSpace = true
            }
        ]
    };
}
