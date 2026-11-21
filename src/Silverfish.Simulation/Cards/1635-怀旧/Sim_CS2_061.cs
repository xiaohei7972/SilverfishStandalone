using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 吸取生命
// Drain Life
// 造成$2点伤害，为你的英雄恢复#2点生命值。
// Deal $2 damage. Restore #2 Health to your hero.
internal static class Sim_CS2_061
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
