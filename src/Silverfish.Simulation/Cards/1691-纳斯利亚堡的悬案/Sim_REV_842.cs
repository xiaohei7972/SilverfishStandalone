using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 晋升
// Promotion
// 使一个白银之手新兵获得+3/+3和嘲讽。
// Give a Silver Hand Recruit +3/+3 and Taunt.
internal static class Sim_REV_842
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_SILVER_HAND_RECRUIT)],
        []);
    // </legacy-play-requirements>

}
