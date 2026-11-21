using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣典籍
// Holy Book
// 沉默并消灭一个随从。召唤一个它的10/10复制。
// Silence and destroy a minion. Summon a 10/10 copy of it.
internal static class Sim_VAC_464t7
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
