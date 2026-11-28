using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 符文宝珠
// Runed Orb
// 造成$2点伤害。发现一张法术牌。
// Deal $2 damage. Discover a spell.
internal static class Sim_BAR_541
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
