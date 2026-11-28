using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 劫持者
// Kidnapper
// 连击：将一个随从移回其拥有者的手牌。
// Combo: Return a minion to_its owner's hand.
[CardSim("NEW1_005")]
internal static class Sim_NEW1_005
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_FOR_COMBO), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ReturnMinionEffect()]
    };
}
