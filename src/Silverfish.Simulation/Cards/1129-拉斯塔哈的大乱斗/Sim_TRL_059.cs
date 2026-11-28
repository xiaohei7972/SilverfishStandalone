using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 沼泽游荡者
// Bog Slosher
// 战吼：将一个友方随从移回你的手牌，并使其获得+2/+2。
// Battlecry: Return a friendly minion to your hand and give it +2/+2.
internal static class Sim_TRL_059
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
