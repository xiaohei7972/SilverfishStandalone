using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 星空投影师
// Celestial Projectionist
// 战吼：选择一个友方随从，将它的一张临时复制置入你的手牌。
// Battlecry: Choose a friendly minion. Add a Temporary copy of it to your hand.
internal static class Sim_TTN_742
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
