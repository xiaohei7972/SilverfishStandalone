using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 撼地巨吼
// Earthen Roar
// 将一个敌方随从的生命值变为1。如果你的手牌中有龙牌，再选择一个。
// Set an enemy minion's Health to 1. If you're holding a Dragon, pick another.
internal static class Sim_CATA_554
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
