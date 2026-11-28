using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 下水道爬行者
// Sewer Crawler
// 战吼：召唤一个2/3的巨鼠。
// Battlecry: Summon a 2/3_Giant Rat.
internal static class Sim_LOOT_069
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
