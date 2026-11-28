using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 蛇发女妖佐拉
// Zola the Gorgon
// 战吼：选择一个友方随从。将它的金色复制置入你的手牌。
// Battlecry: Choose a friendly minion. Add a Golden copy of it to your hand.
internal static class Sim_CORE_LOOT_516
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
