using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 骑士队长
// Knight-Captain
// 战吼：造成3点伤害。荣誉消灭：获得+3/+3。
// Battlecry: Deal 3 damage. Honorable Kill: Gain +3/+3.
internal static class Sim_AV_131
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
