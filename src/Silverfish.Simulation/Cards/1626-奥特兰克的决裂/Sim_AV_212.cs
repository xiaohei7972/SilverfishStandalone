using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 法力虹吸
// Siphon Mana
// 造成$2点伤害。荣誉消灭：使你手牌中所有法术牌的法力值消耗减少（1）点。
// Deal $2 damage. Honorable Kill: Reduce the Cost of spells in your hand by (1).
internal static class Sim_AV_212
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
