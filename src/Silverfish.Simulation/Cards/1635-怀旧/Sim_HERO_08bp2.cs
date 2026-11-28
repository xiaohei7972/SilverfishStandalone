using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 二级火焰冲击
// Fireblast Rank 2
// 英雄技能 造成$2点伤害。
// Hero Power Deal $2 damage.
[CardSim("HERO_08bp2")]
internal static class Sim_HERO_08bp2
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_TARGET_TO_PLAY)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.Selected, 2),
        ],
    };
}
