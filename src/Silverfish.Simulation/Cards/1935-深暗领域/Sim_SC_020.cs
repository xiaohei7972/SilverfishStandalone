using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吞噬精华
// Consume
// 移除一个 友方地标的1点耐久度，为你的英雄恢复#8点生命值。
// Remove 1 Durability from a friendly location to restore #8 Health to your hero.
internal static class Sim_SC_020
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_LOCATION_TARGET)],
        []);
    // </legacy-play-requirements>

}
