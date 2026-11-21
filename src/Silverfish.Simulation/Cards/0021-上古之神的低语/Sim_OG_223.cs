using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣之力
// Divine Strength
// 使一个随从获得+1/+2。
// Give a minion +1/+2.
internal static class Sim_OG_223
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
