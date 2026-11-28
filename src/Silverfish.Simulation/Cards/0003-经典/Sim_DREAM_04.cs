using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 梦境
// Dream
// 将一个随从移回其拥有者的 手牌。
// Return a minion to its owner's hand.
[CardSim("DREAM_04")]
internal static class Sim_DREAM_04
{
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new ReturnMinionEffect()]
    };
}
