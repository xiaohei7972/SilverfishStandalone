using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 列车烟囱
// Smokestack
// 对一个随从造成$1点伤害。如果该随从死亡，发掘一个 宝藏。
// Deal $1 damage to a minion. If it dies, Excavate a treasure.
internal static class Sim_WW_378
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
