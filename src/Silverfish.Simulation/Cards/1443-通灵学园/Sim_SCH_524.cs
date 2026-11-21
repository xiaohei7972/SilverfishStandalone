using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 荣誉护盾
// Shield of Honor
// 使一个受伤的随从获得+3攻击力和圣盾。
// Give a damaged minion +3 Attack and Divine Shield.
internal static class Sim_SCH_524
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_DAMAGED_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
