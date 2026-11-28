using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 紧急冷冻剂
// Emergency Coolant
// 冻结一个随从。
// Freeze a minion.
[CardSim("PART_005")]
internal static class Sim_PART_005
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new FreezeEffect(FreezeTarget.Selected)]
    };
}
