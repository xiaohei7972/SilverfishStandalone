namespace Silverfish.Simulation.Cards;

// 刃牙德鲁伊
// Druid of the Saber
// 抉择：变形成为2/1并具有冲锋；或者变形成为3/2并具有潜行。
// Choose One - Transform into a 2/1 with Charge; or a 3/2 with Stealth.
[CardSim("AT_042")]
internal static class Sim_AT_042
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("AT_042a", [new TransformMinionEffect("AT_042t", Self: true)]),
            new("AT_042b", [new TransformMinionEffect("AT_042t2", Self: true)]),
        ],
        CombinedChoice = new("both", [new TransformMinionEffect("OG_044c", Self: true)]),
    };
}
