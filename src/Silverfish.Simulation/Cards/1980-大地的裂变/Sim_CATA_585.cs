using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 烈火炙烤
// Torch
// 对一个受伤的随从造成$@点伤害。如果伤害超过目标生命值，将能造成剩余伤害的本牌移回手牌。
// Deal $@ damage to a damaged minion. Return this to hand with any excess damage.
internal static class Sim_CATA_585
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_DAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
