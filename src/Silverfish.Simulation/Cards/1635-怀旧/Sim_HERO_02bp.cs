using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 图腾召唤
// Totemic Call
// 英雄技能 随机召唤一个基础图腾。
// Hero Power Summon a random basic Totem.
[CardSim("HERO_02bp")]
internal static class Sim_HERO_02bp
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_ALL_BASIC_TOTEMS_NOT_IN_PLAY), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonRandomMinionEffect(["NEW1_009", "CS2_050", "CS2_051", "CS2_052"]) { ExcludeOnBoard = true },
        ],
    };
}
