using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 可靠陪伴
// Trusty Companion
// 使一个随从获得+2/+3。如果该随从有随从类型，抽一张该类型的随从牌。
// Give a minion +2/+3. If it has a minion type, draw one of that type.
internal static class Sim_WW_027
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
