using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 空间撕裂器
// Dimensional Ripper
// 召唤你的牌库中一个随从的两个复制。
// Summon 2 copies of a minion in your deck.
internal static class Sim_DAL_059
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
