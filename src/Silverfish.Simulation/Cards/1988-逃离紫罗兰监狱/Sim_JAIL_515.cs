using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影轮射
// Shadow Rounds
// 对一个敌方随从造成$2点伤害。如果该随从死亡，随机对另一个敌方随从施放本法术。
// Deal $2 damage to an enemy minion. If it dies, cast this on another random enemy minion.
internal static class Sim_JAIL_515
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
