using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 希尔瓦娜斯的胜利
// Sylvanas's Triumph
// 造成$3点伤害。如果你使用过本牌的其他复制，改为对所有敌人造成伤害。
// Deal $3 damage. If you've played another copy of this, hit all enemies instead.
internal static class Sim_CATA_557
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
