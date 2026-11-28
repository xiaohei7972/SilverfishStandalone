namespace Silverfish.Simulation.Cards;

// 暗言术：骇
// Shadow Word: Horror
// 消灭所有攻击力小于或等于2的随从。
// Destroy all minions with 2 or less Attack.
[CardSim("OG_100")]
internal static class Sim_OG_100
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DestroyMatchingMinionsEffect(2),
        ],
    };
}
