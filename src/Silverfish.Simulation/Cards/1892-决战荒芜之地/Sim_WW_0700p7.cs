using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪能枪弹
// Fel Bullet
// 造成$2点伤害。抽一张牌。每回合 切换。
// Deal $2 damage. Draw a card. Swaps each turn.
internal static class Sim_WW_0700p7
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
