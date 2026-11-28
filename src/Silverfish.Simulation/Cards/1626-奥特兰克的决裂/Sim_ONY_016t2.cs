using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 憎恨之翼（等级3）
// Wings of Hate (Rank 3)
// 召唤四只1/1的邪翼蝠。
// Summon four 1/1 Felwings.
internal static class Sim_ONY_016t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
