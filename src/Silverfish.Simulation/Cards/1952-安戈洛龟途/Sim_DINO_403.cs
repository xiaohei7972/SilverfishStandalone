using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 魔暴龙面具
// Devilsaur Mask
// 将一个随从的属性值变为8/8。使其获得冲锋。
// Set a minion's stats to 8/8. Give it Charge.
internal static class Sim_DINO_403
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
