using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 旅行医者
// Traveling Healer
// 圣盾，战吼：恢复#3点生命值。
// Divine Shield Battlecry: Restore #3 Health.
internal static class Sim_DAL_078
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
