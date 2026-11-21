using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火元素
// Fire Elemental
// 战吼：造成4点伤害。
// Battlecry: Deal 4 damage.
internal static class Sim_CORE_CS2_042
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
