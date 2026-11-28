using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 森林的援助
// The Forest's Aid
// 双生法术 召唤五个2/2的 树人。
// Twinspell Summon five 2/2 Treants.
internal static class Sim_DAL_256
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
