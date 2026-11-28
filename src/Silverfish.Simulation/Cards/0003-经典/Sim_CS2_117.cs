using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 大地之环先知
// Earthen Ring Farseer
// 战吼： 恢复#3点生命值。
// Battlecry: Restore #3_Health.
[CardSim("CS2_117")]
internal static class Sim_CS2_117
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new HealEffect(HealTarget.Selected, 3)]
    };
}
