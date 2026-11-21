using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 幻象制造者
// Mirage Caller
// 战吼：选择一个随从，召唤一个它的1/1复制。
// Battlecry: Choose a minion. Summon a 1/1 copy of it.
internal static class Sim_UNG_022
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
