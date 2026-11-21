namespace Silverfish.Simulation.Cards;

// 翼手龙宝宝
// Pterrordax Hatchling
// 战吼：进化。
// Battlecry: Adapt.
[CardSim("UNG_001")]
internal static class Sim_UNG_001
{
    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new AdaptChoice(AdaptChoice.StandardOptions)
    };
}
