using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 电击
// Zap!
// 对一个随从造成$2点伤害。过载：（1）
// Deal $2 damage to a minion. Overload: (1)
internal static class Sim_GIL_600
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
