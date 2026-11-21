using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 远古映像
// Ancient Reflections
// 选择一个随从。用它的1/1的复制填满你的面板。
// Choose a minion. Fill your board with 1/1 copies of it.
internal static class Sim_VAC_464t19
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
