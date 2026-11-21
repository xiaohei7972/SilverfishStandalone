using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪恶图书馆
// Vile Library
// 使一个友方随从获得+1/+1。你每控制一个小鬼，重复一次。
// Give a friendly minion +1/+1. Repeat for each Imp you control.
internal static class Sim_REV_799
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
