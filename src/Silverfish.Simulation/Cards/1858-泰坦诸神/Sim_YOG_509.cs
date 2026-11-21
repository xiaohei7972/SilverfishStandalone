using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 守护者的力量
// Keeper's Strength
// 使一个友方随从获得+2/+2。对所有其他随从造成等同于其攻击力的伤害。
// Give a friendly minion +2/+2. Deal its Attack damage to all other minions.
internal static class Sim_YOG_509
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
