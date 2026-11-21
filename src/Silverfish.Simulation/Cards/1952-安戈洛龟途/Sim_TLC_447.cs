using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 烧蚀毒雾
// Caustic Fumes
// 消灭一个敌方随从。延系：对所有随从造成$2点伤害。
// Destroy an enemy minion. Kindred: Deal $2 damage to all minions.
internal static class Sim_TLC_447
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
