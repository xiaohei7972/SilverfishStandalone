using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 怒袭
// Bash
// 造成$3点伤害。获得3点 护甲值。
// Deal $3 damage. Gain 3 Armor.
internal static class Sim_AT_064
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
