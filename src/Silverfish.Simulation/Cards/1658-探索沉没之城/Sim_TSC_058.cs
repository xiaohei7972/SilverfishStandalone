using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 捕掠
// Predation
// 造成$2点伤害。如果你在本牌在你手中时使用过纳迦牌，则法力值消耗为（0）点。
// Deal $2 damage. Costs (0) if you played a Naga while holding this.
internal static class Sim_TSC_058
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
