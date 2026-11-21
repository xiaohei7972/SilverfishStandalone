using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 正中要害
// Pressure Points
// 对一个随从造成$3点伤害。使你手牌中的连击牌法力值消耗减少（1）点。
// Deal $3 damage to a minion. Reduce the Cost of Combo cards in your hand by (1).
internal static class Sim_GDB_881
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
