using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 亚煞极之力
// Y'Shaarj's Strength
// 召唤一个2/2的泥浆怪。
// Summon a 2/2 Slime.
internal static class Sim_OG_202a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
