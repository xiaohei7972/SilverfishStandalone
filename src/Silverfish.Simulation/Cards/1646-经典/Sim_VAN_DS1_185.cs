using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奥术射击
// Arcane Shot
// 造成$2点伤害。
// Deal $2 damage.
internal static class Sim_VAN_DS1_185
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
