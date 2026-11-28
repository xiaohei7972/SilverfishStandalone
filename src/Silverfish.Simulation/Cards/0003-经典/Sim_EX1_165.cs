namespace Silverfish.Simulation.Cards;

// 利爪德鲁伊
// Druid of the Claw
// 抉择：变形成为7/6并具有突袭；或者变形成为4/9并具有嘲讽。
// Choose One - Transform into a 7/6 with Rush; or a 4/9 with Taunt.
[CardSim("EX1_165")]
internal static class Sim_EX1_165
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("EX1_165a", [new TransformMinionEffect("EX1_165t1", Self: true)]),
            new("EX1_165b", [new TransformMinionEffect("EX1_165t2", Self: true)])
        ]
    };
}
