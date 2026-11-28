using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 拆迁修理工
// Demolition Renovator
// 可交易 战吼：摧毁一个敌方地标。
// Tradeable Battlecry: Destroy an enemy location.
internal static class Sim_REV_023
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_LOCATION_TARGET)],
        []);
    // </legacy-play-requirements>

}
