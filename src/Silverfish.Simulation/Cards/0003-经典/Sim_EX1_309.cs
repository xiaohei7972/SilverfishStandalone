using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 灵魂虹吸
// Siphon Soul
// 消灭一个随从，为你的英雄恢复#3点生命值。
// Destroy a minion. Restore #3 Health to_your hero.
internal static class Sim_EX1_309
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
