using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 光合作用
// Photosynthesis
// 恢复#6点生命值。随机获取3张德鲁伊法术牌。
// Restore #6 Health. Get 3 random Druid spells.
internal static class Sim_EDR_848
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
