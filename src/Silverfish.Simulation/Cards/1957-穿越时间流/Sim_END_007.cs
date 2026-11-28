using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 发挥优势
// Press the Advantage
// 造成$1点伤害。在本回合中，使你的英雄获得+1攻击力。抽1张牌。获得1点护甲值。
// Deal $1 damage. Give your hero +1 Attack this turn. Draw 1 card. Gain 1 Armor.
internal static class Sim_END_007
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
