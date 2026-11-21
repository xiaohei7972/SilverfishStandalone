using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 无面暴怒者
// Faceless Rager
// 战吼：复制一个友方随从的生命值。
// Battlecry: Copy a friendly minion's Health.
internal static class Sim_DAL_744
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
