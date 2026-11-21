using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 石塘猎人
// Rockpool Hunter
// 战吼：使一个友方鱼人获得+1/+1。
// Battlecry: Give a friendly Murloc +1/+1.
internal static class Sim_UNG_073
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.MURLOC), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
