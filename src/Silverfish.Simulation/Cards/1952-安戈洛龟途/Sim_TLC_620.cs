using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 强固
// Fortify
// 获得3点护甲值。对一个敌方随从造成等同于你护甲值的伤害。
// Gain 3 Armor. Deal damage equal to your Armor to an enemy minion.
internal static class Sim_TLC_620
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
