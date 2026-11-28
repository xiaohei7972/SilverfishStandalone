using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 海草卷击
// Seaweed Strike
// 对一个随从造成$4点伤害。如果你在本牌在你手中时使用过纳迦牌，使你的英雄在本回合中获得+4攻击力。
// Deal $4 damage to a minion. If you played a Naga while holding this, also give your hero +4 Attack this turn.
internal static class Sim_TSC_651
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
