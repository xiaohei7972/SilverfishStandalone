using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 标记射击
// Marked Shot
// 对一个随从造成$4点伤害。发现一张法术牌。
// Deal $4 damage to_a_minion. Discover_a_spell.
internal static class Sim_DAL_371
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
