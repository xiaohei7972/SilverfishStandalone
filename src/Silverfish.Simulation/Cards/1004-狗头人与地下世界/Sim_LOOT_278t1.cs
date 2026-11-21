using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 生命药剂
// Elixir of Life
// 使一个随从获得+2/+2和吸血。
// Give a minion +2/+2 and Lifesteal.
internal static class Sim_LOOT_278t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
