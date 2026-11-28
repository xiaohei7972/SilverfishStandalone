using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 闪电箭
// Lightning Bolt
// 造成$3点伤害，过载：（1）
// Deal $3 damage. Overload: (1)
internal static class Sim_CORE_EX1_238
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
