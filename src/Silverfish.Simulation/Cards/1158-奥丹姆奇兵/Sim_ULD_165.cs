using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 裂隙屠夫
// Riftcleaver
// 战吼：消灭一个随从。你的英雄受到等同于该随从生命值的 伤害。
// Battlecry: Destroy a minion. Your hero takes damage equal to its Health.
internal static class Sim_ULD_165
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
