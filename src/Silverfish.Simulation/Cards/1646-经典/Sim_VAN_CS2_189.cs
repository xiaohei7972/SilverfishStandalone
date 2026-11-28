using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 精灵弓箭手
// Elven Archer
// 战吼：造成1点伤害。
// Battlecry: Deal 1 damage.
internal static class Sim_VAN_CS2_189
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
