using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 加基森名媛
// Gadgetzan Socialite
// 战吼： 恢复#2点生命值。
// Battlecry: Restore #2_Health.
internal static class Sim_CFM_659
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
