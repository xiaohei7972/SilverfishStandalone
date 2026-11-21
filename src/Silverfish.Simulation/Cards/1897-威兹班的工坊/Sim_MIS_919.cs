using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 木偶剧场
// Puppet Theatre
// 选择一个敌方随从，获取一张它的1/1且法力值消耗为（1）点的复制。
// Choose an enemy minion. Get a 1/1 copy of it that costs (1).
internal static class Sim_MIS_919
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
