using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 防腐仪式
// Embalming Ritual
// 使一个随从获得复生。
// Give a minion Reborn.
internal static class Sim_ULD_265
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
