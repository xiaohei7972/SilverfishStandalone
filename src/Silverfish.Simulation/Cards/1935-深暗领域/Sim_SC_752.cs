using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 黑暗圣堂武士
// Dark Templar
// 潜行。战吼：消灭一个敌方随从。再使用一张圣堂武士即可融合为执政官！
// Stealth. Battlecry: Destroy an enemy minion. Play another Templar to merge into an Archon!
internal static class Sim_SC_752
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
