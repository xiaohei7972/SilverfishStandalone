using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 梦魇
// Nightmare
// 使一个随从获得+5/+5，在你的下个回合开始时，消灭该随从。
// Give a minion +5/+5. At the start of your next turn, destroy it.
internal static class Sim_DREAM_05
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
