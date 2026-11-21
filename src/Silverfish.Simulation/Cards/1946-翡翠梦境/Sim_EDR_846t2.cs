using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 已腐蚀的梦境
// Corrupted Dream
// 将一个随从 洗入其拥有者的 牌库。
// Shuffle a minion into its owner's deck.
internal static class Sim_EDR_846t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
