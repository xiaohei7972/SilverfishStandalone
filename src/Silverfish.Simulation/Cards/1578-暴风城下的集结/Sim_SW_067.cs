using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 监狱守卫
// Stockades Guard
// 战吼：使一个友方随从获得嘲讽。
// Battlecry: Give a friendly minion Taunt.
internal static class Sim_SW_067
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
