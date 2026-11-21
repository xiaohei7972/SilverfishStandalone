using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣佳酿
// Divine Brew
// 使一个角色获得圣盾。 （还剩2杯！）
// Give a character Divine Shield. (2 Drinks left!)
internal static class Sim_VAC_916t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
