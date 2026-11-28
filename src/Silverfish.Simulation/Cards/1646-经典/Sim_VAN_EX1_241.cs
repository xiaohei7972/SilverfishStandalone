using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 熔岩爆裂
// Lava Burst
// 造成$5点伤害，过载：（2）
// Deal $5 damage. Overload: (2)
internal static class Sim_VAN_EX1_241
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
