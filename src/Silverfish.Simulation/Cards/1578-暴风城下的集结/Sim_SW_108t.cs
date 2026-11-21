using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 传承之火
// Second Flame
// 对一个随从造成$2点伤害。
// Deal $2 damage to a minion.
internal static class Sim_SW_108t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
