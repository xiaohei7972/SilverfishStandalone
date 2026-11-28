using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 无罪！
// Innocent!
// 在本回合中，使一个随从获得 免疫。
// Give a minion Immune this turn.
internal static class Sim_MAW_025b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
