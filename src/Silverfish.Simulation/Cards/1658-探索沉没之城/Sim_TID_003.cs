using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 迷潮挖掘者
// Tidelost Burrower
// 战吼：探底。如果选中的是鱼人牌，召唤一个它的2/2的复制。
// Battlecry: Dredge. If it's a Murloc, summon a 2/2 copy of it.
internal static class Sim_TID_003
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
