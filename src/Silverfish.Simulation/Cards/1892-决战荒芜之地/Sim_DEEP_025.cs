using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 破碎映像
// Shattered Reflections
// 选择一个随从，将一个它的复制置入你的手牌，牌库和战场。
// Choose a minion. Add a copy to your hand, deck, and battlefield.
internal static class Sim_DEEP_025
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
