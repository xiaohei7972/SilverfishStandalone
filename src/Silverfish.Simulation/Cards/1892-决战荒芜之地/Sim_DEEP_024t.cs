using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 亮石旋岩虫
// Glowstone Gyreworm
// 已锻造 吸血。战吼：造成5点伤害。
// Forged, Lifesteal Battlecry: Deal 5 damage.
internal static class Sim_DEEP_024t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
