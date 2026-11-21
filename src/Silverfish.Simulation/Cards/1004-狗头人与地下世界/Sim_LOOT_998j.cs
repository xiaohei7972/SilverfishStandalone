using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 扎罗格的王冠
// Zarog's Crown
// 发现一张传说随从牌。召唤两个它的复制。
// Discover a Legendary minion. Summon two copies of it.
[CardSim("LOOT_998j")]
internal static class Sim_LOOT_998j
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        ChooseAfterPlay = new DiscoverMinionChoice(SummonCopies: 2, LegendaryOnly: true)
    };
}
