using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 刺骨
// Eviscerate
// 造成$2点伤害；连击：改为造成$4点伤害。
// Deal $2 damage. Combo: Deal $4 damage instead.
[CardSim("EX1_124")]
internal static class Sim_EX1_124
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
            new ConditionalEffect(new ComboCondition(), [new DamageEffect(DamageTarget.Selected, 4)])
            {
                Otherwise = [new DamageEffect(DamageTarget.Selected, 2)]
            }
        ]
    };
}
