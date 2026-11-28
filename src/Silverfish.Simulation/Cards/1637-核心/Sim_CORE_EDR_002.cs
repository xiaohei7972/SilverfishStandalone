using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 毒性吐息
// Poison Breath
// 使一个友方亡灵获得剧毒。
// Give a friendly Undead Poisonous.
internal static class Sim_CORE_EDR_002
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.UNDEAD)],
        []);
    // </legacy-play-requirements>

}
