using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 赞美迦拉克隆
// Praise Galakrond!
// 使一个随从获得+1攻击力。祈求迦拉克隆。
// Give a minion +1 Attack. Invoke Galakrond.
internal static class Sim_DRG_030
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
