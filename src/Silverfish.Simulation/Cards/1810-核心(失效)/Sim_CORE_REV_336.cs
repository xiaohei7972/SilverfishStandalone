using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 罪恶谋划
// Plot of Sin
// 召唤两个2/2的树人。注能（@）：改为两个5/5的古树。
// Summon two 2/2 Treants. Infuse (@): Two 5/5 Ancients instead.
internal static class Sim_CORE_REV_336
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
