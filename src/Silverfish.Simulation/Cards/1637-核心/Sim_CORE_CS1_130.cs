using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣惩击
// Holy Smite
// 对一个随从造成$3点伤害。
// Deal $3 damage to a minion.
internal static class Sim_CORE_CS1_130
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
