using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 苦涩结局
// Bitter End
// 冻结一个随从及其相邻随从，并消灭其中受伤的随从。
// Freeze a minion and its neighbors. Destroy any that are damaged.
internal static class Sim_END_023
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
