using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 狂欢舞台
// Mosh Pit
// 消耗3份残骸，使一个友方随从获得复生。
// Spend 3 Corpses to give a friendly minion Reborn.
internal static class Sim_ETC_533
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [],
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)]);
    // </legacy-play-requirements>

}
