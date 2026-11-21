using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 茶水小弟
// Waterboy
// 战吼： 在本回合中，你的下一个英雄技能的法力值消耗为（0）点。
// Battlecry: Your next Hero Power this turn costs (0).
internal static class Sim_TRL_407
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
