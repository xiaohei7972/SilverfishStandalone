using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣愤怒
// Holy Wrath
// 抽一张牌， 并对一个随从造成等同于该牌法力值消耗的伤害。
// Draw a card and deal damage equal to its Cost to a minion.
internal static class Sim_EX1_365
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
