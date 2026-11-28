using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 毒蛇陷阱
// Snake Trap
// 奥秘：当你的随从受到攻击时，召唤三条1/1的蛇。
// Secret: When one of your minions is attacked, summon three 1/1 Snakes.
[CardSim("EX1_554")]
internal static class Sim_EX1_554
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AttackProposed, TriggerRelation.Friendly,
                [new SummonEffect("EX1_554t"), new SummonEffect("EX1_554t"), new SummonEffect("EX1_554t")])
            {
                Focus = TriggerFocus.AttackDefender, EntityKind = EntityKind.Minion, RequiresBoardSpace = true
            }
        ]
    };
}
