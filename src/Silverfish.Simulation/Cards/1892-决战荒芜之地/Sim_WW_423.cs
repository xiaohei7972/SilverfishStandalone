using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 沙龙酒仙
// Saloon Brewmaster
// 战吼：将一个友方随从移回你的手牌，使其获得+2/+2。
// Battlecry: Return a friendly minion to your hand. Give it +2/+2.
internal static class Sim_WW_423
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
