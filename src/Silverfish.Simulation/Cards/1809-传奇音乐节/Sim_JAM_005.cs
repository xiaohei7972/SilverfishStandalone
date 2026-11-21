using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 约德尔狂吼歌手
// Yelling Yodeler
// 战吼： 触发一个友方随从的 亡语，触发两次。
// Battlecry: Trigger a friendly minion's Deathrattle twice.
internal static class Sim_JAM_005
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
