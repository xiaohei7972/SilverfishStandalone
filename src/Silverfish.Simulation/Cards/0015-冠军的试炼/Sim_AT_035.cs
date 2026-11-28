namespace Silverfish.Simulation.Cards;

// 危机四伏
// Beneath the Grounds
// 将三张伏击牌洗入你对手的牌库。当你的对手抽到该牌，便为你召唤一个4/4的蛛魔。
// Shuffle 3 Ambushes into your opponent's deck. When drawn, you summon a 4/4 Nerubian.
[CardSim("AT_035")]
internal static class Sim_AT_035
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateCardEffect("AT_035t", Count: 3, Placement: CardPlacement.ShuffleIntoDeck, Recipient: CardRecipient.Opponent),
        ],
    };
}
