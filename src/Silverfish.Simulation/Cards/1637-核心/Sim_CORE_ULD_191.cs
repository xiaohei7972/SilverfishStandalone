using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 欢快的同伴
// Beaming Sidekick
// 战吼：使一个友方随从获得+2生命值。
// Battlecry: Give a friendly minion +2 Health.
internal static class Sim_CORE_ULD_191
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
