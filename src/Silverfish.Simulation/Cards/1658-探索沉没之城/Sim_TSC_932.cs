using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 血染大海
// Blood in the Water
// 对一个敌人造成$3点伤害。召唤一条5/5并具有突袭的鲨鱼。
// Deal $3 damage to an enemy. Summon a 5/5 Shark with Rush.
internal static class Sim_TSC_932
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
