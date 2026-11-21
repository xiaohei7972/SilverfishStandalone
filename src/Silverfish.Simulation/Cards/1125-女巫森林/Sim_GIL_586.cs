using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 大地之力
// Earthen Might
// 使一个随从获得+2/+2。如果该随从是元素，则随机将一张元素牌置入你的手牌。
// Give a minion +2/+2. If it's an Elemental, add a random Elemental to your hand.
internal static class Sim_GIL_586
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
