using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 缚苔熊怪
// Furbolg Mossbinder
// 战吼：将一个友方随从变形成为一个6/6的元素。
// Battlecry: Transform a friendly minion into a 6/6_Elemental.
internal static class Sim_LOOT_150
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
