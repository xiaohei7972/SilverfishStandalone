namespace Silverfish.Simulation.Cards;

// 米米尔隆的头部
// Mimiron's Head
// 在你的回合开始时，如果你控制至少三个机械，则消灭这些机械，并将其组合成V-07-TR-0N。
// At the start of your turn, if you have at least 3 Mechs, destroy them all and form V-07-TR-0N.
[CardSim("GVG_111")]
internal static class Sim_GVG_111
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnStarted,
                TriggerRelation.Friendly,
                [new MimironCombineEffect("GVG_111t", 3)])
        ]
    };
}
