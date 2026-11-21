using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 烟雾熏蒸
// Fumigate
// 对一个随从及所有相同类型的其他随从造成$3点伤害。
// Deal $3 damage to a minion and all others of the same minion type.
internal static class Sim_TLC_901
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
