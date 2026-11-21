using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 背刺
// Backstab
// 对一个未受伤的随从造成$2点 伤害。
// Deal $2 damage to an undamaged minion.
internal static class Sim_VAN_CS2_072
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_UNDAMAGED_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
