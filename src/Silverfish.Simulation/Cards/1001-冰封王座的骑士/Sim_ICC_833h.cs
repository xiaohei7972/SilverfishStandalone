using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰冷触摸
// Icy Touch
// 造成$1点伤害。如果该英雄技能消灭了一个随从，则召唤一个水元素。
// Deal $1 damage. If this kills a minion, summon a Water Elemental.
internal static class Sim_ICC_833h
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
