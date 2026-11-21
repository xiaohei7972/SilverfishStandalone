using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 树群来袭
// TREEEES!!!
// 选择一个随从。召唤四个2/2的树人攻击该随从。
// Choose a minion. Summon four 2/2 Treants that attack it.
internal static class Sim_TLC_230
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
