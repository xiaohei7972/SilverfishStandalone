namespace Silverfish.Simulation.Cards;

// 火车王里诺艾
// Leeroy Jenkins
// 冲锋，战吼： 为你的对手召唤两条1/1的雏龙。
// Charge. Battlecry: Summon two 1/1 Whelps for your opponent.
[CardSim("EX1_116")]
internal static class Sim_EX1_116
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonEffect("EX1_116t", Recipient: CardRecipient.Opponent),
            new SummonEffect("EX1_116t", Recipient: CardRecipient.Opponent)
        ]
    };
}
