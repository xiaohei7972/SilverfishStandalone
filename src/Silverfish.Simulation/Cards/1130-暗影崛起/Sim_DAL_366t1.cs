using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 刺客合约
// Assassin's Contract
// 消灭一个随从。召唤一个1/2的耐心的刺客。
// Destroy a minion. Summon a 1/2 Patient Assassin.
internal static class Sim_DAL_366t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
