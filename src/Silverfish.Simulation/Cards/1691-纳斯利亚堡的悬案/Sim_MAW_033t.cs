using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 被告希尔瓦娜斯
// Sylvanas, the Accused
// 已注能 战吼：夺取一个敌方随从的控制权。
// Infused Battlecry: Take control of an enemy minion.
internal static class Sim_MAW_033t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
