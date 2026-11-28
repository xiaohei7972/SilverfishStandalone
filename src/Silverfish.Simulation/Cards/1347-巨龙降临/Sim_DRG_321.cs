using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火球滚滚
// Rolling Fireball
// 对一个随从造成$8点伤害，超过其生命值的伤害将由左侧或右侧的随从承担。
// Deal $8 damage to a minion. Any excess damage continues to the left or right.
internal static class Sim_DRG_321
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
