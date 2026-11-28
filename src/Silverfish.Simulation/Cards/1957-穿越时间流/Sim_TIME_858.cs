using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 时空构造体
// Temporal Construct
// 战吼：对一个敌方随从造成5点伤害，然后抽牌，数量等同于超过目标生命值的伤害。
// Battlecry: Deal 5 damage to an enemy minion. Draw cards equal to the excess damage.
internal static class Sim_TIME_858
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
