using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 审判
// Judgment
// 预备。选择一个友方随从，将所有随从的属性值变为与该随从相同。
// Prepare Choose a friendly minion. Set all minions' stats equal to that minion's.
internal static class Sim_JAIL_326
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
