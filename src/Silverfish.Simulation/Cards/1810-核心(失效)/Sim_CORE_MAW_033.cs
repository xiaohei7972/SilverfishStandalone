using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 被告希尔瓦娜斯
// Sylvanas, the Accused
// 战吼：消灭一个敌方随从。注能（@）：改为夺取其控制权。
// Battlecry: Destroy an enemy minion. Infuse (@): Take control of it instead.
internal static class Sim_CORE_MAW_033
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
