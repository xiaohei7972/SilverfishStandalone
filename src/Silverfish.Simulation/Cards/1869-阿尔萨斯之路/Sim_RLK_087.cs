using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 窒息
// Asphyxiate
// 消灭攻击力最高的敌方随从。
// Destroy the highest Attack enemy minion.
internal static class Sim_RLK_087
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1)],
        []);
    // </legacy-play-requirements>

}
