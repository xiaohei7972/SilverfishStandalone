using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 惩黑除恶
// Stand Against Darkness
// 召唤五个{0}的白银之手新兵。
// Summon five {0} Silver Hand Recruits.
[CardSim("OG_273")]
internal static class Sim_OG_273
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonEffect("CS2_101t"),
            new SummonEffect("CS2_101t"),
            new SummonEffect("CS2_101t"),
            new SummonEffect("CS2_101t"),
            new SummonEffect("CS2_101t"),
        ],
    };
}
