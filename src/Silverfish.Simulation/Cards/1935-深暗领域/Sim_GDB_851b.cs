using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 晕击星光
// Stunning Star
// 选择一个敌方随从。使其休眠2回合。
// Choose an enemy minion. It goes Dormant for 2 turns.
internal static class Sim_GDB_851b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
