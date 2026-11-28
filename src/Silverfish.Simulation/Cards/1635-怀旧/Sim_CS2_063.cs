using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 腐蚀术
// Corruption
// 选择一个敌方随从，在你的回合开始时，消灭该随从。
// Choose an enemy minion. At the start of your turn, destroy it.
internal static class Sim_CS2_063
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
