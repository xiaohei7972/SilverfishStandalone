using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火元素
// Fire Elemental
// 战吼：造成3点伤害。
// Battlecry: Deal 3 damage.
internal static class Sim_VAN_CS2_042
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
