using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 急速冷却
// Cold Snap
// 冻结一个敌人。随机获取一张冰霜法术牌。
// Freeze an enemy. Get a random Frost spell.
internal static class Sim_JAIL_125
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_NONSTEALTH_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
