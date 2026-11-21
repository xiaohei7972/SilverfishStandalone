using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 乌索尔印记
// Mark of Ursol
// 选择一个随从。如果是敌方随从，将其属性值变为1/1；如果是友方随从，改为将其属性值变为3/3。
// Choose a minion. If it's an enemy, set its stats to 1/1. If it's friendly, set its stats to 3/3 instead.
internal static class Sim_EDR_252
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
