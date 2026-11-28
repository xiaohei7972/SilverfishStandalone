namespace Silverfish.Simulation.Cards;

// 罗宁
// Rhonin
// 亡语：将三张奥术飞弹置入你的手牌。
// Deathrattle: Add 3 copies of Arcane Missiles to your hand.
[CardSim("AT_009")]
internal static class Sim_AT_009
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new CreateCardEffect("EX1_277", Count: 3),
        ],
    };
}
