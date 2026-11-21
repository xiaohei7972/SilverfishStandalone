namespace Silverfish.Simulation.Cards;

// 火山龙
// Volcanosaur
// 战吼：连续进化两次。
// Battlecry: Adapt, then Adapt.
[CardSim("UNG_002")]
internal static class Sim_UNG_002
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new AdaptChoice(AdaptChoice.StandardOptions, Times: 2)
    };
}
