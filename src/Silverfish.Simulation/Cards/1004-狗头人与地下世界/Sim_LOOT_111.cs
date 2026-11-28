using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 机械异种蝎
// Scorp-o-matic
// 战吼： 消灭一个攻击力小于或等于1的随从。
// Battlecry: Destroy a minion with 1 or less Attack.
internal static class Sim_LOOT_111
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MAX_ATTACK, 1), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
