using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 永时困苦
// Eternal Toil
// 对一个随从造成$1点伤害。如果该随从依然存活，抽一张牌。如果该随从死亡，随机召唤一个法力值消耗为（1）的随从。
// Deal $1 damage to a minion. If it survives, draw a card. If it dies, summon a random 1-Cost minion.
internal static class Sim_END_020
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
