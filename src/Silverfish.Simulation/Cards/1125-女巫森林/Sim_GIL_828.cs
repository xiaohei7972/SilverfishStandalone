using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 凶猛狂暴
// Dire Frenzy
// 使一个野兽获得+3/+3。将它的三张复制洗入你的牌库，且这些复制都具有+3/+3。
// Give a Beast +3/+3. Shuffle 3 copies into your deck with +3/+3.
internal static class Sim_GIL_828
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_WITH_RACE, TAG_RACE.BEAST), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
