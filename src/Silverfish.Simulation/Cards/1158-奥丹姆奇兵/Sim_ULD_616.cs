using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 泰坦造物跟班
// Titanic Lackey
// 战吼：使一个友方随从获得+2生命值和 嘲讽。
// Battlecry: Give a friendly minion +2 Health and_Taunt.
internal static class Sim_ULD_616
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
