using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 铁肩冲撞
// Shoulder Check
// 可交易 使一个随从获得+2/+1和突袭。
// Tradeable Give a minion +2/+1 and Rush.
internal static class Sim_ONY_025
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
