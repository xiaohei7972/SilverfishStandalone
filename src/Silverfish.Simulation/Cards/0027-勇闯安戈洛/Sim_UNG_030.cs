using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 联结治疗
// Binding Heal
// 为你的英雄和一个随从恢复#5点生命值。
// Restore #5 Health to a minion and your hero.
internal static class Sim_UNG_030
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
