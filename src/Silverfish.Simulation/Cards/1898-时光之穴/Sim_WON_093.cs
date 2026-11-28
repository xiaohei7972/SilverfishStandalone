using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恶魔融合
// Demonfuse
// 使一个恶魔获得+3/+3。
// Give a Demon +3/+3.
internal static class Sim_WON_093
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.DEMON)],
        []);
    // </legacy-play-requirements>

}
