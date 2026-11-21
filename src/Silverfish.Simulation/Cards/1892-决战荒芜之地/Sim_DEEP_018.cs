using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 魔菇采掘
// Shroomscavate
// 使一个随从获得圣盾。发掘一个宝藏。
// Give a minion Divine Shield. Excavate a treasure.
internal static class Sim_DEEP_018
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
