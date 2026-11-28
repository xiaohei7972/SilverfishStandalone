using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 流水祈咒
// Water Invocation
// 为所有友方角色恢复6点生命值。
// Restore 6 Health to all friendly characters.
internal static class Sim_AV_258t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
