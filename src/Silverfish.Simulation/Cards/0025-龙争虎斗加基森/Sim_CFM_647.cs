using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吹箭鱼人
// Blowgill Sniper
// 战吼：造成1点伤害。
// Battlecry: Deal 1 damage.
internal static class Sim_CFM_647
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
