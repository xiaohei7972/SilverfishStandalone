namespace Silverfish.Simulation.Cards;

// 比斯巨兽
// The Beast
// 亡语： 为你的对手召唤一个3/3的皮普·急智。
// Deathrattle: Summon a 3/3 Pip Quickwit for your opponent.
[CardSim("EX1_577")]
internal static class Sim_EX1_577
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new SummonEffect("EX1_finkle", Recipient: CardRecipient.Opponent)]
    };
}
