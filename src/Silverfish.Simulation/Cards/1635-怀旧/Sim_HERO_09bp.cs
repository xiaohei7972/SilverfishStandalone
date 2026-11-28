using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 次级治疗术
// Lesser Heal
// 英雄技能 恢复#2点生命值。
// Hero Power Restore #2 Health.
[CardSim("HERO_09bp")]
internal static class Sim_HERO_09bp
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_TARGET_TO_PLAY)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new HealEffect(HealTarget.Selected, 2),
        ],
    };
}
