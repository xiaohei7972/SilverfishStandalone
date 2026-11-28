using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 雷霆震击
// Lightning Jolt
// 造成$2点伤害。
// Deal $2 damage.
[CardSim("AT_050t")]
internal static class Sim_AT_050t
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
            new DamageEffect(DamageTarget.Selected, 2),
        ],
    };
}
