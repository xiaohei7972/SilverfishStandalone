using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 满月辉光
// Light of the Full Moon
// 使一个随从获得+3/+3。将一张新月辉光置入你的手牌。
// Give a minion +3/+3. Add a Light of the New Moon to your hand.
internal static class Sim_FIR_918t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
