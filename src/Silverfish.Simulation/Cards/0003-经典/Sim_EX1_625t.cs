using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 心灵尖刺
// Mind Spike
// 造成$2点伤害。
// Deal $2 damage.
[CardSim("EX1_625t")]
internal static class Sim_EX1_625t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new DamageEffect(DamageTarget.Selected, 2)]
    };
}
