using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 堕落残阳祭司
// Fallen Sun Cleric
// 战吼：使一个友方随从获得+1/+1。
// Battlecry: Give a friendly minion +1/+1.
internal static class Sim_CORE_ICC_094
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
