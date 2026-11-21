using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 清冷音韵
// Chill Vibes
// 恢复#8点生命值。压轴：召唤一个3/3并具有嘲讽的元素。
// Restore #8 Health. Finale: Summon a 3/3 Elemental with Taunt.
internal static class Sim_ETC_369
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
