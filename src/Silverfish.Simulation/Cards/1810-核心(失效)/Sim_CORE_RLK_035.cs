using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪爆
// Corpse Explosion
// 引爆一份残骸，对所有随从造成$1点伤害。如果有随从存活，重复此效果。
// Detonate a Corpse to deal $1 damage to all minions. If any are still alive, repeat this.
internal static class Sim_CORE_RLK_035
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINIMUM_TOTAL_MINIONS, 1), new(ErrorType.REQ_MINIMUM_CORPSES, 1)],
        []);
    // </legacy-play-requirements>

}
