using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 净化的光耀之子
// Cleansing Lightspawn
// 吸血。战吼：对一个敌方随从造成等同于本随从生命值的伤害。
// Lifesteal Battlecry: Deal damage to an enemy minion equal ___to this minion's Health.
internal static class Sim_TIME_427
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
