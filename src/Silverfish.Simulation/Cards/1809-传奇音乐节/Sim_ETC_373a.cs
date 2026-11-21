using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 花朵力量
// Flower Power
// 召唤五个2/2的树人。
// Summon five 2/2 Treants.
internal static class Sim_ETC_373a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
