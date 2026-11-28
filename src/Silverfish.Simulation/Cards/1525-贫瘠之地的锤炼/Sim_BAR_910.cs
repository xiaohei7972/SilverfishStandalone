using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 牺牲魔典
// Grimoire of Sacrifice
// 消灭一个友方随从。对所有敌方随从造成$2点伤害。
// Destroy a friendly minion. Deal $2 damage to all enemy minions.
internal static class Sim_BAR_910
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET)],
        []);
    // </legacy-play-requirements>

}
