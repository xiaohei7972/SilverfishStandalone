using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 嘉沃顿的故事
// Story of Galvadon
// 使一个随从获得三项随机额外效果。
// Give a minion three random Bonus Effects.
internal static class Sim_TLC_444
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
