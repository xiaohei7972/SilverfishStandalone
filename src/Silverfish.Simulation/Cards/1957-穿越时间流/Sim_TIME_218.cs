using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 静电震击
// Static Shock
// 对一个随从造成$1点伤害。在本回合中，使你的英雄获得+1攻击力。
// Deal $1 damage to a minion. Give your hero +1 Attack this turn.
internal static class Sim_TIME_218
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
