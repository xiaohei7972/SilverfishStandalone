using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 一脚踢飞
// Knockback
// 对一个随从造成$@点伤害（你每将卡牌洗入牌库一次都会提升）。
// Deal $@ damage to a minion (improved for each time you've shuffled cards into your deck).
internal static class Sim_TLC_517
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
