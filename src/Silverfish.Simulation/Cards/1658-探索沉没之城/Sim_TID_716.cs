using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 潮汐亡魂
// Tidal Revenant
// 战吼：造成5点伤害。获得8点护甲值。
// Battlecry: Deal 5 damage. Gain 8 Armor.
internal static class Sim_TID_716
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
