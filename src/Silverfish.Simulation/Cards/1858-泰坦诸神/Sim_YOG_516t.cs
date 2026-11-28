using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 混沌统治
// Reign of Chaos
// 夺取一个敌方随从的控制权。
// Take control of an enemy minion.
internal static class Sim_YOG_516t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
