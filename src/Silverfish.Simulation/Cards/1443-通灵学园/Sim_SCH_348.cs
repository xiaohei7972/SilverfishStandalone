using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 燃烧
// Combustion
// 对一个随从造成$4点伤害，相邻的随从均会受到超过其生命值的伤害。
// Deal $4 damage to a minion. Any excess damages both neighbors.
internal static class Sim_SCH_348
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
