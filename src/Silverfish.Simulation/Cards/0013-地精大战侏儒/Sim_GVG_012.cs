using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 纳鲁之光
// Light of the Naaru
// 恢复#3点生命值。如果目标仍处于受伤状态，则召唤一个圣光护卫者。
// Restore #3 Health. If the target is still damaged, summon a Lightwarden.
internal static class Sim_GVG_012
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
