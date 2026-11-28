using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 希望圣契
// Libram of Hope
// 恢复#8点生命值。召唤一个8/8并具有嘲讽和圣盾的 守卫。
// Restore #8 Health. Summon an 8/8 Guardian with Taunt and_Divine Shield.
internal static class Sim_BT_024
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
