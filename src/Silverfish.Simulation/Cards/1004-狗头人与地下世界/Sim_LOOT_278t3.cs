using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影药剂
// Elixir of Shadows
// 使一个随从获得+2/+2，并召唤一个1/1复制。
// Give a minion +2/+2. Summon a 1/1 copy of_it.
internal static class Sim_LOOT_278t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
