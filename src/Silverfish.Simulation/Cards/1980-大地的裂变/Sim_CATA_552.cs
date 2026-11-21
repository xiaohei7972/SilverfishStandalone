using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 乌鳞斥候
// Ebonscale Scout
// 战吼：造成等同于本随从攻击力的伤害。（当本牌在你手中时，使用一张龙牌即可将本牌变为8/8的龙！）
// Battlecry: Deal damage equal to this minion's Attack. (While in hand, play a Dragon to become an 8/8 Dragon!)
internal static class Sim_CATA_552
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
