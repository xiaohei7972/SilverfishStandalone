using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 可靠的灯泡
// Faithful Lumi
// 战吼：使一个友方机械获得+1/+1。
// Battlecry: Give a friendly Mech +1/+1.
internal static class Sim_BOT_079
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.MECHANICAL), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
