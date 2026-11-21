using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 宇宙具象
// Cosmic Manifestations
// 造成$2点伤害。随机将一张恶魔猎手法术牌洗入你的牌库。流放：重复一次。
// Deal $2 damage. Shuffle a random Demon Hunter spell into your deck. Outcast: Do it again.
internal static class Sim_JAIL_892
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
