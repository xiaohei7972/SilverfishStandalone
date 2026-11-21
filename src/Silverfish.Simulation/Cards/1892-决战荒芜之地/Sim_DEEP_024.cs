using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 亮石旋岩虫
// Glowstone Gyreworm
// 吸血。快枪：造成5点伤害。锻造：将快枪变为战吼。
// Lifesteal Quickdraw: Deal 5 damage. Forge: Change Quickdraw to Battlecry.
internal static class Sim_DEEP_024
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
