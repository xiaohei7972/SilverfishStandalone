using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 守护者的呼唤
// Call the Guardians
// +5/+5并具有嘲讽。
// +5/+5 and Taunt.
internal static class Sim_GVG_041a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
