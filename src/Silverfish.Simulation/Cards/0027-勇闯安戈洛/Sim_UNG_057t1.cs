using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 刀瓣
// Razorpetal
// 造成$2点伤害。
// Deal $2 damage.
internal static class Sim_UNG_057t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
