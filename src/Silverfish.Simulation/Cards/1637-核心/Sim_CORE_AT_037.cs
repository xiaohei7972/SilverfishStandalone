using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 活体根须
// Living Roots
// 抉择：造成$2点伤害；或者召唤两个1/1的树苗。
// Choose One - Deal $2 damage; or Summon two 1/1 Saplings.
internal static class Sim_CORE_AT_037
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
