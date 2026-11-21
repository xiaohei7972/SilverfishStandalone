using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰风暴（等级1）
// Flurry (Rank 1)
// 随机冻结一个敌方随从。（当你有5点法力值时升级。）
// Freeze a random enemy minion. (Upgrades when you have 5 Mana.)
internal static class Sim_BAR_305
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
