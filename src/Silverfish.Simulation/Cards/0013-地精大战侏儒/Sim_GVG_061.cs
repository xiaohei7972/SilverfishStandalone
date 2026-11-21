using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 作战动员
// Muster for Battle
// 召唤三个{0}的白银之手新兵，装备一把1/4的武器。
// Summon three {0} Silver Hand Recruits. Equip a 1/4 Weapon.
internal static class Sim_GVG_061
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
