using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑暗附体
// Dark Possession
// 对一个友方角色造成$2点伤害。发现一张恶魔牌。
// Deal $2 damage to a friendly character. Discover a Demon.
internal static class Sim_GIL_543
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
