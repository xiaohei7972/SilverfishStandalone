using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 焚烧
// Conflagrate
// 对一个随从造成$5点伤害，其拥有者抽一张牌。
// Deal $5 damage to a minion. Its owner draws a card.
internal static class Sim_FIR_954
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
