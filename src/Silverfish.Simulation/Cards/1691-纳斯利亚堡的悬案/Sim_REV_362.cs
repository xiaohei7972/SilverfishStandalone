using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 城堡狗舍
// Castle Kennels
// 使一个友方随从获得+2攻击力。如果是野兽，还会使其获得突袭。
// Give a friendly minion +2 Attack. If it's a Beast, give it Rush.
internal static class Sim_REV_362
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
