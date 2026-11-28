using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 神圣泉水
// Holy Springwater
// 为一个受伤的角色恢复#8点生命值。将超过目标生命值上限的治疗量存入法力值消耗为（1）的瓶子。
// Restore #8 Health to a damaged character. Save any excess in a 1-Cost Bottle.
internal static class Sim_WW_395
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_DAMAGED_TARGET)],
        []);
    // </legacy-play-requirements>

}
