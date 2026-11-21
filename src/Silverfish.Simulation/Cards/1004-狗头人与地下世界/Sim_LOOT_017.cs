using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑暗契约
// Dark Pact
// 消灭一个友方随从。为你的英雄恢复#8点生命值。
// Destroy a friendly minion. Restore #8 Health to your hero.
internal static class Sim_LOOT_017
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
