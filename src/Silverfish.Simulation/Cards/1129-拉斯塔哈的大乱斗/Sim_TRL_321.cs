using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 毁灭打击
// Devastate
// 对一个受伤的随从造成$4点 伤害。
// Deal $4 damage to a damaged minion.
internal static class Sim_TRL_321
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
