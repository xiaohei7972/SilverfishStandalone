using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 邪恶召唤
// Vile Call
// 召唤两个2/2并具有吸血的 恶魔。
// Summon two 2/2 Demons with Lifesteal.
internal static class Sim_BAR_327
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
