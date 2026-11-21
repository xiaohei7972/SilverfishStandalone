using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 毒鳍鱼人
// Toxfin
// 战吼：使一个友方鱼人获得剧毒。
// Battlecry: Give a friendly Murloc Poisonous.
internal static class Sim_DAL_077
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.MURLOC)],
        []);
    // </legacy-play-requirements>

}
