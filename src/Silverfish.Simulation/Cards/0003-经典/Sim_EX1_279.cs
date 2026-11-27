using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 炎爆术
// Pyroblast
// 造成$10点伤害。
// Deal $10 damage.
[CardSim("EX1_279")]
internal static class Sim_EX1_279
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 10)]
    };
}
