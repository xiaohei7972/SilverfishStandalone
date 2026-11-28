using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 老旧的火把
// Forgotten Torch
// 造成$3点伤害。将一张可造成6点伤害的“炽烈的火把”洗入你的牌库。
// Deal $3 damage. Shuffle a 'Roaring Torch' into your deck that deals 6 damage.
[CardSim("LOE_002")]
internal static class Sim_LOE_002
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
            new DamageEffect(DamageTarget.Selected, 3),
            new CreateCardEffect("LOE_002t", Count: 1, Placement: CardPlacement.ShuffleIntoDeck),
        ],
    };
}
