namespace Silverfish.Simulation.Cards;

// 蛛魔伏击
// Nerubian Ambush!
// 抽到时施放 为你的对手召唤一个4/4的蛛魔。
// Casts When Drawn Summon a 4/4 Nerubian for your opponent.
[CardSim("AT_035t")]
internal static class Sim_AT_035t
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonEffect("FP1_007t", Recipient: CardRecipient.Opponent),
        ],
    };
}
