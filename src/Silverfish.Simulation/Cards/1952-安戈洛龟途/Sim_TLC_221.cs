using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 炽火缠身
// Sizzling Swarm
// 造成$3点伤害，召唤相同数量的2/1的炽烈烬火。
// Deal $3 damage. Summon that many 2/1 Sizzling Cinders.
internal static class Sim_TLC_221
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
