using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 布巴
// Bubba
// 战吼：召唤六只 1/1并具有突袭的血猎犬并使其攻击一个 敌方随从。
// Battlecry: Summon six 1/1 Bloodhounds with Rush to attack an enemy minion.
internal static class Sim_VAC_464t6
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
