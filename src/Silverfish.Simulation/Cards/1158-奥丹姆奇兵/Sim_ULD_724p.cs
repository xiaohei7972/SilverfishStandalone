using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 方尖碑之眼
// Obelisk's Eye
// 恢复#3点生命值。如果你的目标是一个随从，则同时使其获得+3/+3。
// Restore #3 Health. If you target a minion, also give it +3/+3.
internal static class Sim_ULD_724p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
