using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 湮灭
// Obliterate
// 消灭一个随从。对你的英雄造成3点伤害。
// Destroy a minion. Deal_3 damage to your hero.
internal static class Sim_LEG_RLK_125
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
