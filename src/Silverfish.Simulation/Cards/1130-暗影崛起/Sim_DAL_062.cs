using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 横扫攻击
// Sweeping Strikes
// 使一个随从获得 “同时对其攻击目标相邻的随从造成伤害。”
// Give a minion "Also damages minions next to whomever this attacks."
internal static class Sim_DAL_062
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
