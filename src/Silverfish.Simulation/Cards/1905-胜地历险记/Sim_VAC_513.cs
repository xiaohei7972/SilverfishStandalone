using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 滑坡
// Slippery Slope
// 冻结一个角色。每有一个被冻结的角色，抽一张牌。
// Freeze a character. Draw a card for each Frozen character.
internal static class Sim_VAC_513
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
