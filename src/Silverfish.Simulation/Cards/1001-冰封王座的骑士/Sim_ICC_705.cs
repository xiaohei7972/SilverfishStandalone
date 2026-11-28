using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 骨魇
// Bonemare
// 战吼：使一个友方随从获得+4/+4和嘲讽。
// Battlecry: Give a friendly minion +4/+4 and Taunt.
internal static class Sim_ICC_705
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
