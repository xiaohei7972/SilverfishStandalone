using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 不测之变
// Eventuality
// 造成$2点伤害。灌注你的英雄技能。
// Deal $2 damage. Imbue your Hero Power.
internal static class Sim_END_000
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
