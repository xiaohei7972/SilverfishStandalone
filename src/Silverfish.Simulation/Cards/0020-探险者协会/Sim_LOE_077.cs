namespace Silverfish.Simulation.Cards;

// 布莱恩·铜须
// Brann Bronzebeard
// 你的战吼会触发 两次。
// Your Battlecries trigger twice.
[CardSim("LOE_077")]
internal static class Sim_LOE_077
{
    internal static CardRules Rules { get; } = new()
    {
        DoubleFriendlyBattlecries = true,
    };
}
