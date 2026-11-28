using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 利刺荆棘
// Piercing Thorns
// 对一个随从造成$2点伤害。
// Deal $2 damage to a minion.
internal static class Sim_DAL_350a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
