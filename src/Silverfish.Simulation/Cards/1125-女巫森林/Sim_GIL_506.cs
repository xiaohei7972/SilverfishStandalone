using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 偷袭
// Cheap Shot
// 回响 对一个随从造成 $2点伤害。
// Echo Deal $2 damage to a_minion.
internal static class Sim_GIL_506
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
