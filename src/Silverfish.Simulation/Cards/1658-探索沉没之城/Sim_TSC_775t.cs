using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 沉没的祭仪
// Sunken Ritual
// 沉默一个随从，并召唤两个它的复制。
// Silence a minion and summon 2 copies of it.
internal static class Sim_TSC_775t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
