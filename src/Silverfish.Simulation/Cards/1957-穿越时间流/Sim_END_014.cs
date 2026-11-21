using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 协作火花
// Synchronized Spark
// 对一个敌人造成$3点伤害。如果该角色死亡，随机使一个友方随从获得+3/+3。
// Deal $3 damage to an enemy. If it dies, give a random friendly minion +3/+3.
internal static class Sim_END_014
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
