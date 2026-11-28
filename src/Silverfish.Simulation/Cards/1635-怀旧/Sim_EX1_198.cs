using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 娜塔莉·塞林
// Natalie Seline
// 战吼：消灭一个随从并获得其生命值。
// Battlecry: Destroy a minion and gain its Health.
internal static class Sim_EX1_198
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
