using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 不稳定的邪能箭
// Unstable Felbolt
// 对一个敌方随从和一个随机友方随从造成$3点伤害。
// Deal $3 damage to an enemy minion and a random friendly one.
internal static class Sim_BT_199
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
