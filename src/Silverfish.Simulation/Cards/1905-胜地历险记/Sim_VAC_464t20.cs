using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 香蕉分裂
// Banana Split
// 使一个友方随从获得+2/+2，并召唤两个它的复制。
// Give a friendly minion +2/+2. Summon two copies of it.
internal static class Sim_VAC_464t20
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
