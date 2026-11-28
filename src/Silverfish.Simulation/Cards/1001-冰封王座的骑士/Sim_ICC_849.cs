using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 拥抱黑暗
// Embrace Darkness
// 选择一个敌方随从。在你的回合开始时，获得该随从的 控制权。
// Choose an enemy minion. At the start of your turn, gain control of it.
internal static class Sim_ICC_849
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
