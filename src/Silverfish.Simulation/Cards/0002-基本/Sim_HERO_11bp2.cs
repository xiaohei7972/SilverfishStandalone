using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 食尸鬼狂暴
// Ghoul Frenzy
// 英雄技能 召唤一个2/1并具有冲锋的食尸鬼。它会在回合结束时死亡。
// Hero Power Summon a 2/1 Ghoul with Charge. It dies at end of turn.
[CardSim("HERO_11bp2")]
internal static class Sim_HERO_11bp2
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_NUM_MINION_SLOTS, 1)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonEffect("HERO_11bp2t"),
        ],
    };
}
