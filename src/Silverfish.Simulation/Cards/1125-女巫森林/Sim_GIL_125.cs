using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 疯帽客
// Mad Hatter
// 战吼：随机向其他随从丢出三顶帽子。每顶帽子可使随从获得+1/+1。
// Battlecry: Randomly toss 3 hats to other minions. Each hat gives +1/+1.
internal static class Sim_GIL_125
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.UNDEAD), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
