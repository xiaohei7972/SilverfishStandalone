using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 连环爆裂
// Crackle
// 造成$3到$6点伤害，过载：（1）
// Deal $3-$6 damage. Overload: (1)
internal static class Sim_GVG_038
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
