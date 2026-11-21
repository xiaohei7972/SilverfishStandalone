using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 年迈的酒仙
// Ancient Brewmaster
// 战吼：使一个友方随从从战场上移回你的手牌。
// Battlecry: Return a friendly minion from the battlefield to your hand.
internal static class Sim_VAN_EX1_057
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
