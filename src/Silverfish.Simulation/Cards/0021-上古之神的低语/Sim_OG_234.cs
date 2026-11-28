using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 夜色镇炼金师
// Darkshire Alchemist
// 战吼： 恢复#5点生命值。
// Battlecry: Restore #5 Health.
[CardSim("OG_234")]
internal static class Sim_OG_234
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
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
