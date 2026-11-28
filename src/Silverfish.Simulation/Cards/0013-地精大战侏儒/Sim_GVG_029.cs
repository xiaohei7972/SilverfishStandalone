namespace Silverfish.Simulation.Cards;

// 先祖召唤
// Ancestor's Call
// 每个玩家从手牌中随机将一个随从置入战场。
// Put a random minion from each player's hand into the battlefield.
[CardSim("GVG_029")]
internal static class Sim_GVG_029
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonFromHandEffect(Recipient: CardRecipient.Controller),
            new SummonFromHandEffect(Recipient: CardRecipient.Opponent)
        ]
    };
}
