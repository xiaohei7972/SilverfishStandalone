using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 复苏
// Renew
// 恢复#3点 生命值。发现一张法术牌。
// Restore #3 Health. Discover a spell.
internal static class Sim_BT_252
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
