using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 英勇圣印
// Seal of Champions
// 使一个随从获得+3攻击力和 圣盾。
// Give a minion +3 Attack and Divine Shield.
internal static class Sim_AT_074
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
