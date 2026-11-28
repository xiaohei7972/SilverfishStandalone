using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 假期规划
// Vacation Planning
// 恢复#4点生命值。召唤3个白银之手新兵。抽两张牌。
// Restore #4 Health. Summon 3 Silver Hand Recruits. Draw 2 cards.
internal static class Sim_WORK_003
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
