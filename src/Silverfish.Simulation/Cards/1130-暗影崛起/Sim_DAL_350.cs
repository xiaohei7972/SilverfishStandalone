using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 水晶之力
// Crystal Power
// 抉择：对一个随从造成$2点伤害；或者恢复#5点生命值。
// Choose One - Deal $2 damage to a minion; or_Restore #5 Health.
internal static class Sim_DAL_350
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
