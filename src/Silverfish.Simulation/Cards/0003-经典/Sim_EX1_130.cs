namespace Silverfish.Simulation.Cards;

// 崇高牺牲
// Noble Sacrifice
// 奥秘：当一个敌人攻击时，召唤一个2/1的防御者，并使其成为攻击的目标。
// Secret: When an enemy attacks, summon a 2/1 Defender as the new target.
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
