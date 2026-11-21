using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 尖壳印记
// Mark of the Spikeshell
// 使一个随从获得+2/+2。如果该随从拥有嘲讽，则将该随从的一张复制置入你的手牌。
// Give a minion +2/+2. If it has Taunt, add a copy of it to your hand.
internal static class Sim_BAR_549
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
