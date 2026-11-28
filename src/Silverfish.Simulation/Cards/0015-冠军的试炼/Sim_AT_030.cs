using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 幽暗城勇士
// Undercity Valiant
// 连击：造成1点伤害。
// Combo: Deal 1 damage.
[CardSim("AT_030")]
internal static class Sim_AT_030
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_FOR_COMBO)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.Selected, 1),
        ],
    };
}
