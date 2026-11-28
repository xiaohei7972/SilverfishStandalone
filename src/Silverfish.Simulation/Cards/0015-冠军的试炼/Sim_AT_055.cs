using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 快速治疗
// Flash Heal
// 恢复#5点生命值。
// Restore #5 Health.
[CardSim("AT_055")]
internal static class Sim_AT_055
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new HealEffect(HealTarget.Selected, 5),
        ],
    };
}
