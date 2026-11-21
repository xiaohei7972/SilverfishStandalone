using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 噬渊律师
// Attorney-at-Maw
// 抉择：沉默一个随从；或者使一个随从在本回合中获得免疫。
// Choose One - Silence a minion; or Give a minion Immune this turn.
internal static class Sim_CORE_MAW_025
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
