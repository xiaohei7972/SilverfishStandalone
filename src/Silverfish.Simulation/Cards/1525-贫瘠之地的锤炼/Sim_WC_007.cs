using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 毒蛇花
// Serpentbloom
// 使一只友方野兽获得剧毒。
// Give a friendly Beast Poisonous.
internal static class Sim_WC_007
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST)],
        []);
    // </legacy-play-requirements>

}
