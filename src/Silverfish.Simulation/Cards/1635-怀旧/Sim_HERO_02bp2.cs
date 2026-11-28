using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 图腾崇拜
// Totemic Slam
// 英雄技能 召唤一个你想要的图腾。
// Hero Power Summon a Totem of your choice.
[CardSim("HERO_02bp2")]
internal static class Sim_HERO_02bp2
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_NUM_MINION_SLOTS, 1)], []);

    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("AT_132_SHAMANa", [new SummonEffect("AT_132_SHAMANa")]),
            new("AT_132_SHAMANb", [new SummonEffect("AT_132_SHAMANb")]),
            new("AT_132_SHAMANc", [new SummonEffect("AT_132_SHAMANc")]),
            new("AT_132_SHAMANd", [new SummonEffect("AT_132_SHAMANd")]),
        ],
        DoNotCombineChoices = true,
    };
}
