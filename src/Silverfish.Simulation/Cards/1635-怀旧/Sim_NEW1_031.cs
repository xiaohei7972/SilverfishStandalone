using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 动物伙伴
// Animal Companion
// 随机召唤一个野兽伙伴。
// Summon a random Beast Companion.
[CardSim("NEW1_031")]
internal static class Sim_NEW1_031
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonRandomMinionEffect(["NEW1_032", "NEW1_033", "NEW1_034"])]
    };
}
