using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 阴暗的人影
// Shadowy Figure
// 战吼： 变形成为一个友方亡语随从的2/2复制。
// Battlecry: Transform into a_2/2 copy of a friendly Deathrattle minion.
internal static class Sim_DAL_030
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
