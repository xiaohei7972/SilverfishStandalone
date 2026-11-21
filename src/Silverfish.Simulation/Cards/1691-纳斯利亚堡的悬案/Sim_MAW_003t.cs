using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 图腾物证
// Totemic Evidence
// 已注能 召唤全部4个基础图腾。
// Infused Summon all 4 basic Totems.
internal static class Sim_MAW_003t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
