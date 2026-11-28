using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 火焰祈咒
// Fire Invocation
// 对敌方英雄造成$6点伤害。每回合切换。
// Deal $6 damage to the enemy hero. Swaps each turn.
internal static class Sim_AV_258pt3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
