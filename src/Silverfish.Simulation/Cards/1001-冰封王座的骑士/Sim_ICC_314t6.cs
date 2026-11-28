using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 湮灭
// Obliterate
// 消灭一个随从。你的英雄受到等同于该随从生命值的 伤害。
// Destroy a minion. Your hero takes damage equal to its Health.
internal static class Sim_ICC_314t6
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
