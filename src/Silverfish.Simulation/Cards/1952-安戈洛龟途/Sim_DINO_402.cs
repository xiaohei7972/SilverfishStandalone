using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 蝙蝠面具
// Bat Mask
// 将一个非传说友方随从的属性值变为1/1。用它的复制填满你的面板。
// Set a friendly non-Legendary minion's stats to 1/1. Fill your board with copies of it.
internal static class Sim_DINO_402
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
