using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵魂炸弹
// Spirit Bomb
// 对一个随从和你的英雄各造成$4点伤害。
// Deal $4 damage to a minion and your hero.
internal static class Sim_BOT_222
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
