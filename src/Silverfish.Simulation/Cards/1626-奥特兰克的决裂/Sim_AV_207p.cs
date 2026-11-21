using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣之触
// Holy Touch
// 恢复#5点生命值。每回合翻转。
// Restore #5 Health. Flip each turn.
internal static class Sim_AV_207p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
