using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 星辰领主
// Starlord
// 对一个随从造成$5点伤害。
// Deal $5 damage to a minion.
internal static class Sim_VAN_NEW1_007b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
