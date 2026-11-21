using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 死亡翻滚
// Death Roll
// 消灭一个敌方随从。造成等同于其攻击力的伤害，随机分配到所有敌人身上。
// Destroy an enemy minion. Deal damage equal to its Attack randomly split among all enemies.
internal static class Sim_VAC_416
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
