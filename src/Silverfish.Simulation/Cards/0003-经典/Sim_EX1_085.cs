using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 精神控制技师
// Mind Control Tech
// 战吼：如果你的对手有4个或者更多随从，夺取其中一个的控制权。
// Battlecry: If your opponent has 4 or more minions, take control of one.
[CardSim("EX1_085")]
internal static class Sim_EX1_085
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(
                new ZoneCountCondition(ConditionPlayer.Opponent, GameZone.Play, NumberComparison.AtLeast, 4, EntityKind.Minion),
                [new TakeControlRandomEnemyMinionEffect()])
        ]
    };
}
