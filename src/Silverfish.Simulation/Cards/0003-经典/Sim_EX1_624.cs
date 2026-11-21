using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣之火
// Holy Fire
// 造成$5点伤害。为你的英雄恢复#5点生命值。
// Deal $5 damage. Restore #5 Health to your hero.
internal static class Sim_EX1_624
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
