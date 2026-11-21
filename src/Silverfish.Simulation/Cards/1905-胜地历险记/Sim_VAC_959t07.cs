using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 守卫护符
// Amulet of Warding
// 造成$6点伤害。（仅可对随从！）
// Deal $6 damage. (To a minion!)
internal static class Sim_VAC_959t07
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
