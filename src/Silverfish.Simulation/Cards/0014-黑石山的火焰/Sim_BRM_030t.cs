using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 扫尾
// Tail Swipe
// 造成$4点伤害。
// Deal $4 damage.
[CardSim("BRM_030t")]
internal static class Sim_BRM_030t
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
            new DamageEffect(DamageTarget.Selected, 4)
        ]
    };
}
