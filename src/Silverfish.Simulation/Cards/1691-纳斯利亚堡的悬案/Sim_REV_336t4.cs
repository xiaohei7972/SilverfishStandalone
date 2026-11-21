using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 罪恶谋划
// Plot of Sin
// 已注能 召唤两个5/5的古树。
// Infused Summon two 5/5 Ancients.
internal static class Sim_REV_336t4
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
