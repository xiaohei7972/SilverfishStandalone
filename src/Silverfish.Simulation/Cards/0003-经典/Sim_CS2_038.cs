using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 先祖之魂
// Ancestral Spirit
// 使一个随从获得“亡语：再次召唤本随从。”
// Give a minion "Deathrattle: Resummon this minion."
internal static class Sim_CS2_038
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
