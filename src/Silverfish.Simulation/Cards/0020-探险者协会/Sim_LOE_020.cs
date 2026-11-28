namespace Silverfish.Simulation.Cards;

// 大漠沙驼
// Desert Camel
// 战吼：从双方的牌库中各将一个法力值消耗为（1）的随从置入战场。
// Battlecry: Put a 1-Cost minion from each deck into the battlefield.
[CardSim("LOE_020")]
internal static class Sim_LOE_020
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new RecruitEffect() { CardFilter = new CardFilter { MinimumPrintedCost = 1, MaximumPrintedCost = 1 } },
            new RecruitEffect(Recipient: CardRecipient.Opponent) { CardFilter = new CardFilter { MinimumPrintedCost = 1, MaximumPrintedCost = 1 } },
        ],
    };
}
