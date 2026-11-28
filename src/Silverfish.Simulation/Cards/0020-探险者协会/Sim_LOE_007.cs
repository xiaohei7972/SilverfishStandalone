namespace Silverfish.Simulation.Cards;

// 拉法姆的诅咒
// Curse of Rafaam
// 使你的对手获得一张“诅咒”。在对手的回合开始时，如果它在对手的手牌中，则造成2点伤害。
// Give your opponent a 'Cursed!' card. While they hold it, they take 2 damage on their turn.
[CardSim("LOE_007")]
internal static class Sim_LOE_007
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CreateCardEffect("LOE_007t", Count: 1, Recipient: CardRecipient.Opponent),
        ],
    };
}
