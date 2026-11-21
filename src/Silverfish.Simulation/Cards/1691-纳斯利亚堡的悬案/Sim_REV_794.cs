using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大厅
// Great Hall
// 将一个随从的攻击力和生命值变为3。
// Set a minion's Attack and Health to 3.
internal static class Sim_REV_794
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
