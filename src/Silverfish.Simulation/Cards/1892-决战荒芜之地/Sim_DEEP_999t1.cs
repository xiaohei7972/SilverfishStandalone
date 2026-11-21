using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 心灵之花
// Heartblossom
// 使一个友方随从获得+2/+2。 随机对一个敌方 随从造成$2点伤害。
// Give a friendly minion +2/+2. Deal $2 damage to a random enemy minion.
internal static class Sim_DEEP_999t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
