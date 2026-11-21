using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恐龙学
// Dinomancy
// 使一只野兽获得+3/+3。
// Give a Beast +3/+3.
internal static class Sim_UNG_917t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST)],
        []);
    // </legacy-play-requirements>

}
