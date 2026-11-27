namespace Silverfish.Simulation.Cards;

// 烈焰德鲁伊
// Druid of the Flame
// 抉择：变形成为5/2的随从；或者变形成为2/5的随从。
// Choose One - Transform into a 5/2 minion; or a 2/5 minion.
[CardSim("BRM_010")]
internal static class Sim_BRM_010
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new(
                "BRM_010a",
                [new TransformMinionEffect("BRM_010t", Self: true)]),
            new(
                "BRM_010b",
                [new TransformMinionEffect("BRM_010t2", Self: true)])
        ]
    };
}
