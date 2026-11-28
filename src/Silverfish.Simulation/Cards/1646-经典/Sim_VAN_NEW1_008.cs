using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 知识古树
// Ancient of Lore
// 抉择：抽两张牌；或者恢复#5点生命值。
// Choose One - Draw 2 cards; or Restore #5 Health.
internal static class Sim_VAN_NEW1_008
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
