using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 封印命运
// Seal Fate
// 对一个未受伤的角色造成$3点伤害。祈求迦拉克隆。
// Deal $3 damage to an undamaged character. Invoke Galakrond.
internal static class Sim_DRG_247
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_UNDAMAGED_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
