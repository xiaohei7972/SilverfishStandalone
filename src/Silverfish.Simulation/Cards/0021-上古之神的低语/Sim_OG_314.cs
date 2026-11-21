using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 化血为脓
// Blood To Ichor
// 对一个随从造成$1点伤害，如果它依然存活，则召唤一个2/2的泥浆怪。
// Deal $1 damage to a minion. If it survives, summon a 2/2 Slime.
internal static class Sim_OG_314
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
