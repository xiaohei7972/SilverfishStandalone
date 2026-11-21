using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 陨石术
// Meteor
// 对一个随从造成$15点伤害，并对其相邻的随从造成 $4点伤害。
// Deal $15 damage to a minion and $4 damage to adjacent ones.
internal static class Sim_UNG_955
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
