using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 石头
// Rock
// 造成$3点伤害。
// Deal $3 damage.
internal static class Sim_WW_001t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
