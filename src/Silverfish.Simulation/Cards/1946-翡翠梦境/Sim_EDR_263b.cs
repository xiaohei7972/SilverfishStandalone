using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 巨狼的指引
// Greatwolf's Guidance
// 召唤两只3/2并具有突袭的狼。
// Summon two 3/2 Wolves with Rush.
internal static class Sim_EDR_263b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
