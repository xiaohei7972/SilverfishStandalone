using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 投喂加餐
// Chow Down
// 召唤五条5/4的亡灵幼龙。消耗8份残骸，使其获得突袭。
// Summon five 5/4 Undead Drakes. Spend 8 Corpses to give them Rush.
internal static class Sim_CATA_465
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
