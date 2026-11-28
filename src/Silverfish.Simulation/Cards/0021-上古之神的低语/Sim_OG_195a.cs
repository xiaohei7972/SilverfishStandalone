using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小精灵大军
// Many Wisps
// 召唤七个1/1的小精灵。
// Summon seven 1/1 Wisps.
internal static class Sim_OG_195a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
