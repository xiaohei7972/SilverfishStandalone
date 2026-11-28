namespace Silverfish.Simulation.Cards;

// 末日守卫
// Doomguard
// 冲锋。战吼：&随机弃两张牌。
// Charge. Battlecry:& Discard two random cards.
[CardSim("EX1_310")]
internal static class Sim_EX1_310
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DiscardEffect(2)]
    };
}
