using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 扫出历史
// Strike from History
// 选择两个敌方随从，将其移出 对战。
// Choose two enemy minions. Remove them from the game.
internal static class Sim_TTN_429t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
