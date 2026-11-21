using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 终极感染
// Ultimate Infestation
// 造成$5点伤害。抽五张牌。获得5点护甲值。召唤一个5/5的食尸鬼。
// Deal $5 damage. Draw 5 cards. Gain 5 Armor. Summon a 5/5 Ghoul.
internal static class Sim_ICC_085
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
