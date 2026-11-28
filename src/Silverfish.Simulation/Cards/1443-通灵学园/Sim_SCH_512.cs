using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 通窍
// Initiation
// 对一个随从造成$4点伤害。如果该随从死亡，召唤一个新的复制。
// Deal $4 damage to a minion. If it dies, summon a new copy.
internal static class Sim_SCH_512
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
