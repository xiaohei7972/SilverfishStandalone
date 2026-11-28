using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 治疗术
// Heal
// 英雄技能 恢复#4点生命值。
// Hero Power Restore #4 Health.
[CardSim("HERO_09bp2")]
internal static class Sim_HERO_09bp2
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_TARGET_TO_PLAY)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new HealEffect(HealTarget.Selected, 4),
        ],
    };
}
