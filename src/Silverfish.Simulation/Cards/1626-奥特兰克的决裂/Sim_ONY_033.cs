using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小鬼侵染
// Impfestation
// 每有一个敌方随从，便召唤一个3/3的恐惧小鬼并使其攻击对应敌方随从。
// Summon a 3/3 Dread Imp to attack each enemy minion.
internal static class Sim_ONY_033
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_ENEMY_MINIONS, 1), new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
