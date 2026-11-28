using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 开赛集结
// Rally!
// 复活法力值消耗为（1），（2），（3）的友方随从 各一个。
// Resurrect a friendly 1-Cost, 2-Cost, and 3-Cost minion.
internal static class Sim_YOP_009
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
