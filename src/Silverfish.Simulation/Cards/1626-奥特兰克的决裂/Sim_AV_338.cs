using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 坚守桥梁
// Hold the Bridge
// 使一个随从获得+2/+1和圣盾。直到回合结束，使其获得吸血。
// Give a minion +2/+1 and Divine Shield. It gains Lifesteal until end of turn.
internal static class Sim_AV_338
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
