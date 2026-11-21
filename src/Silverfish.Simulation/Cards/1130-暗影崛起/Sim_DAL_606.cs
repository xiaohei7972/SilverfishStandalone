using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 怪盗天才
// EVIL Genius
// 战吼：消灭一个友方随从，随机将两张跟班牌置入你的手牌。
// Battlecry: Destroy a friendly minion to add 2 random Lackeys_to_your_hand.
internal static class Sim_DAL_606
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
