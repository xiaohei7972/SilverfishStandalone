using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 怒袭
// Bash
// 造成$3点伤害。获得3点 护甲值。
// Deal $3 damage. Gain 3 Armor.
[CardSim("AT_064")]
internal static class Sim_AT_064
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
            new GainArmorEffect(3),
        ],
    };
}
