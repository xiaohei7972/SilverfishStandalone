using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 精神控制
// Mind Control
// 夺取一个敌方随从的控制权。
// Take control of an enemy minion.
internal static class Sim_VAN_CS1_113
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_NUM_MINION_SLOTS, 1), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
