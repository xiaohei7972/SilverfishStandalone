using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 萨索瓦尔
// Sathrovarr
// 战吼：选择一个友方随从。将一个它的复制置入你的手牌，牌库以及战场。
// Battlecry: Choose a friendly minion. Add a copy of it to_your hand, deck, and battlefield.
internal static class Sim_DRG_402
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
