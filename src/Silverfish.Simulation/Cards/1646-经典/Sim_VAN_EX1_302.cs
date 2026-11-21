using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 死亡缠绕
// Mortal Coil
// 对一个随从造成$1点伤害。如果该随从死亡，抽一张牌。
// Deal $1 damage to a minion. If it dies, draw a card.
internal static class Sim_VAN_EX1_302
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
