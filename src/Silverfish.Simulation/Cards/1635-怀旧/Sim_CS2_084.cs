using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 猎人印记
// Hunter's Mark
// 使一个随从的生命值变为1。
// Change a minion's Health to 1.
internal static class Sim_CS2_084
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
