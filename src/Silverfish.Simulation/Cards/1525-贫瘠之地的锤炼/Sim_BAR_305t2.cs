using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰风暴（等级3）
// Flurry (Rank 3)
// 随机冻结三个敌方随从。
// Freeze three random enemy minions.
internal static class Sim_BAR_305t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
