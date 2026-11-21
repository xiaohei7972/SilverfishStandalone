using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑暗裁决
// Dark Conviction
// 将一个随从的攻击力和生命值 变为3。
// Set a minion's Attack and Health to 3.
internal static class Sim_ICC_039
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
