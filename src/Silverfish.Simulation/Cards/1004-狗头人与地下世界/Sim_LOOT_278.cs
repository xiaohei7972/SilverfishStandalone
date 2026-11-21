using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 未鉴定的药剂
// Unidentified Elixir
// 使一个随从获得+2/+2。在你手牌中时获得额外效果。
// Give a minion +2/+2. Gains a bonus effect in_your hand.
internal static class Sim_LOOT_278
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
