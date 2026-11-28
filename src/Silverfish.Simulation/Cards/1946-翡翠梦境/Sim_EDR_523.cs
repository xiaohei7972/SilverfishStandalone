using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 欺诈之网
// Web of Deception
// 将一个友方随从移回你的手牌以召唤一只4/4并具有潜行的蜘蛛。
// Return a friendly minion to your hand to summon a 4/4 Spider with Stealth.
internal static class Sim_EDR_523
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
