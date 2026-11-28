using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 先祖治疗
// Ancestral Healing
// 为一个随从恢复所有生命值并使其获得嘲讽。
// Restore a minion to full Health and give it Taunt.
internal static class Sim_VAN_CS2_041
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
