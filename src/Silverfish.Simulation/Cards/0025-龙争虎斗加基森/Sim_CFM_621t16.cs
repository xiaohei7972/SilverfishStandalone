using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火焰之心
// Heart of Fire
// 造成$5点伤害。
// Deal $5 damage.
internal static class Sim_CFM_621t16
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
