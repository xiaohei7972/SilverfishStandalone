using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰风暴（等级2）
// Flurry (Rank 2)
// 随机冻结两个敌方随从。（当你有10点法力值时升级。）
// Freeze two random enemy minions. (Upgrades when you have 10 Mana.)
internal static class Sim_BAR_305t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
