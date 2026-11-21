using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 野性之心
// Heart of the Wild
// 使一个随从获得+2/+2，然后使你的野兽获得+1/+1。
// Give a minion +2/+2, then give your Beasts +1/+1.
internal static class Sim_AV_292
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
