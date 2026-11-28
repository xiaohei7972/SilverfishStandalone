using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 极紫外破坏者
// Ultraviolet Breaker
// 战吼：对一个 敌方随从造成3点伤害。将3张小行星洗入你的牌库。
// Battlecry: Deal 3 damage to an enemy minion. Shuffle 3 Asteroids into your deck.
internal static class Sim_GDB_901
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
