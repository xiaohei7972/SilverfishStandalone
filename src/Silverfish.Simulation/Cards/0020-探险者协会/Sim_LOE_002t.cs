using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 炽烈的火把
// Roaring Torch
// 造成$6点伤害。
// Deal $6 damage.
internal static class Sim_LOE_002t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
