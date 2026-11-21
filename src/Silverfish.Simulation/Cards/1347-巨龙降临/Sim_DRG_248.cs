using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 霜之祈咒
// Invocation of Frost
// 冻结一个敌人。祈求迦拉克隆。
// Freeze an enemy. Invoke Galakrond.
internal static class Sim_DRG_248
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
