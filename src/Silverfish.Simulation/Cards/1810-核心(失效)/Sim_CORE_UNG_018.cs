using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 烈焰喷涌
// Flame Geyser
// 造成$2点伤害。 将一张1/2的元素牌置入你的手牌。
// Deal $2 damage. Add a 1/2 Elemental to_your hand.
internal static class Sim_CORE_UNG_018
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
