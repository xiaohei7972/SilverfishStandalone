using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 森林赠礼
// Forest's Gift
// 你每控制一个随从，使一个友方随从获得+1/+1。
// Give a friendly minion +1/+1 for each minion you control.
internal static class Sim_CATA_138
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
