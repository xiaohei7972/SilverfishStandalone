using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 心灵分裂
// Psyche Split
// 使一个随从获得+1/+2，并召唤一个它的复制。
// Give a minion +1/+2. Summon a copy of it.
internal static class Sim_BT_253
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
