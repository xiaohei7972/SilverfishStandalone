using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 猛禽狂怒
// Furious Fowls
// 选择一个敌人。召唤两只3/3并具有攻击时免疫的小鸟，攻击选中的敌人。
// Choose an enemy. Summon two 3/3 Birds with Immune while attacking to attack it.
internal static class Sim_VAC_410
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
