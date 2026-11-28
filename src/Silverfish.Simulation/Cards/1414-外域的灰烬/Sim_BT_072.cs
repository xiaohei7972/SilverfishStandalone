using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 深度冻结
// Deep Freeze
// 冻结一个敌人。召唤两个3/6的水元素。
// Freeze an enemy. Summon two 3/6 Water Elementals.
internal static class Sim_BT_072
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
