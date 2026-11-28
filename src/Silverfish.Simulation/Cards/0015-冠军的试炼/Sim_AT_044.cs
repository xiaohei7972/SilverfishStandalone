using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 腐根
// Mulch
// 消灭一个随从。随机将一张随从牌置入对手的手牌。
// Destroy a minion. Add a random minion to your opponent's hand.
[CardSim("AT_044")]
internal static class Sim_AT_044
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DestroyMinionEffect(),
            new CreateRandomMatchingCardEffect(new CardFilter { Kind = EntityKind.Minion }, Recipient: CardRecipient.Opponent),
        ],
    };
}
