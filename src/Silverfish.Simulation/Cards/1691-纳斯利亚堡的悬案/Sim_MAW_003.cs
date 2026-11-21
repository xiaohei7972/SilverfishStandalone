using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 图腾物证
// Totemic Evidence
// 选择一个基础图腾并召唤它。 注能（@个图腾）：改为召唤全部4个。
// Choose a basic Totem and summon it. Infuse (@ |4(Totem, Totems)): Summon all 4 instead.
internal static class Sim_MAW_003
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
