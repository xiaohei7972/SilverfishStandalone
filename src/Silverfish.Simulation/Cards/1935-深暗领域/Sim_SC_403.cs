using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 星港
// Starport
// 召唤一个2/1并具有一项发射时效果的星舰组件。
// Summon a 2/1 Starship_Piece with an effect when launched.
internal static class Sim_SC_403
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)]);
    // </legacy-play-requirements>

}
