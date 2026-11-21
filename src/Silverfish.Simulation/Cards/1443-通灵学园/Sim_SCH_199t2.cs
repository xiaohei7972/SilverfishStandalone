using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 转校生
// Transfer Student
// 战吼：造成2点伤害。
// Battlecry: Deal 2 damage.
internal static class Sim_SCH_199t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
