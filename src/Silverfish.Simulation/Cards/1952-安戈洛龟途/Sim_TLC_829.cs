using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 饥饿的魔暴龙
// Ravenous Devilsaur
// 战吼：消灭一个随从。延系：获得被消灭随从的属性值。
// Battlecry: Destroy a minion. Kindred: Gain its stats.
internal static class Sim_TLC_829
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
