using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 真言术：触
// Power Word: Tentacles
// 使一个随从获得+2/+6。
// Give a minion +2/+6.
internal static class Sim_OG_094
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
