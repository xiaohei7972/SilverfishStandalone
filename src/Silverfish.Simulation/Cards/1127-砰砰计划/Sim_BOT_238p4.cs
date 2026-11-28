using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 无人运输机
// Delivery Drone
// 发现一张机械牌。每回合切换。
// Discover a Mech. Swaps_each_turn.
internal static class Sim_BOT_238p4
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_HAND_NOT_FULL)],
        []);
    // </legacy-play-requirements>

}
