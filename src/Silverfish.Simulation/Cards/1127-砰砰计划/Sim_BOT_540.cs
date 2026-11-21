using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 电磁脉冲特工
// E.M.P. Operative
// 战吼： 消灭一个机械。
// Battlecry: Destroy a Mech.
internal static class Sim_BOT_540
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.MECHANICAL)],
        []);
    // </legacy-play-requirements>

}
