using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 白银之手
// The Silver Hand
// 英雄技能 召唤两个{0}的白银之手新兵。
// Hero Power Summon two {0} Recruits.
[CardSim("HERO_04bp2")]
internal static class Sim_HERO_04bp2
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_NUM_MINION_SLOTS, 1)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonEffect("CS2_101t"),
            new SummonEffect("CS2_101t"),
        ],
    };
}
