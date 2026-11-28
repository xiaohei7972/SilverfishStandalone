using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 狂热铸魂者
// Manic Soulcaster
// 战吼：选择一个友方随从，将它的一张复制洗入你的牌库。
// Battlecry: Choose a friendly minion. Shuffle a copy into your deck.
internal static class Sim_CFM_660
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
