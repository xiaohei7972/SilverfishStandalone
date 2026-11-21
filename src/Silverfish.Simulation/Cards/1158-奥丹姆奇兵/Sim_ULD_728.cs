using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 制伏
// Subdue
// 将一个随从的攻击力和生命值变为1。
// Set a minion's Attack and Health to 1.
internal static class Sim_ULD_728
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
