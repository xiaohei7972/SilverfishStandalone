using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 兽群呼唤
// Call of the Wild
// 召唤全部三个动物伙伴。
// Summon all three Animal Companions.
[CardSim("OG_211")]
internal static class Sim_OG_211
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_NUM_MINION_SLOTS, 1)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonEffect("NEW1_032"),
            new SummonEffect("NEW1_033"),
            new SummonEffect("NEW1_034"),
        ],
    };
}
