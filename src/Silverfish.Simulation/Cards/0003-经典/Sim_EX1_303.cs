using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗影烈焰
// Shadowflame
// 消灭一个友方随从，对所有敌方随从造成等同于其攻击力的伤害。
// Destroy a friendly minion and deal its Attack damage to all enemy minions.
internal static class Sim_EX1_303
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
