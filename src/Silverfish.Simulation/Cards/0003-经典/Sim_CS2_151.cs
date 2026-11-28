namespace Silverfish.Simulation.Cards;

// 白银之手骑士
// Silver Hand Knight
// 战吼：召唤一个2/2的侍从。
// Battlecry: Summon a 2/2_Squire.
[CardSim("CS2_151")]
internal static class Sim_CS2_151
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonEffect("CS2_152")]
    };
}
