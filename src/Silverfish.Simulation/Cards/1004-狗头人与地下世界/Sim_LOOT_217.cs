using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 来我身边
// To My Side!
// 召唤一个动物伙伴，如果你的牌库里没有随从牌，则召唤两个。
// Summon an Animal Companion, or 2 if your deck has no minions.
internal static class Sim_LOOT_217
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
