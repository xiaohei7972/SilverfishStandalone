using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 月火术
// Moonfire
// 造成2点伤害。
// Deal 2 damage.
internal static class Sim_EX1_166a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
