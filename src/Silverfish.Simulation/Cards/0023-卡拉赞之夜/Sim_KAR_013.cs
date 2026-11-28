using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 净化
// Purify
// 沉默一个友方随从，抽一张牌。
// Silence a friendly minion. Draw a card.
[CardSim("KAR_013")]
internal static class Sim_KAR_013
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SilenceEffect(), new DrawEffect(1)],
    };
}
