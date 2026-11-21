using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 临终之息
// Final Gasp
// 对一个随从造成$1点伤害。如果该随从死亡，召唤一个2/2并具有随机效果的冒险者。
// Deal $1 damage to a minion. If it dies, summon a 2/2 Adventurer with a random bonus effect.
internal static class Sim_WC_022
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
