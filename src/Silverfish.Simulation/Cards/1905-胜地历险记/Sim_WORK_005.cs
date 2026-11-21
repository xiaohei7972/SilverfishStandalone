using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 快刀快递
// Sharp Shipment
// 使你的武器获得+2/+2。
// Give your weapon +2/+2.
internal static class Sim_WORK_005
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_WEAPON_EQUIPPED)],
        []);
    // </legacy-play-requirements>

}
