using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 心脏打击
// Heart Strike
// 对一个随从造成$3点伤害。如果该随从死亡，获得一份残骸。
// Deal $3 damage to _a minion. If it dies, gain a Corpse.
internal static class Sim_RLK_034
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
