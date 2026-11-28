using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 影焰晕染
// Shadowflame Suffusion
// 造成$2点伤害。发现一张具有黑暗之赐的战士随从牌。
// Deal $2 damage. Discover a Warrior minion with a Dark Gift.
internal static class Sim_FIR_939
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
