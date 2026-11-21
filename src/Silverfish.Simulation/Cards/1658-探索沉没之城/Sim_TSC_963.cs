using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 鱼排斗士
// Filletfighter
// 战吼：造成1点伤害。
// Battlecry: Deal 1 damage.
internal static class Sim_TSC_963
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
