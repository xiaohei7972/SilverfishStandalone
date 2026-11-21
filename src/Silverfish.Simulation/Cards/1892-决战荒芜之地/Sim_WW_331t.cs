using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 蛇油
// Snake Oil
// 可交易 造成$0点伤害。好像没什么用……
// Tradeable Deal $0 damage. This seems useless...
internal static class Sim_WW_331t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
