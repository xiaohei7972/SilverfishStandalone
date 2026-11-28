using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 心灵碎裂
// Mind Shatter
// 造成$3点伤害。
// Deal $3 damage.
internal static class Sim_EX1_625t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
