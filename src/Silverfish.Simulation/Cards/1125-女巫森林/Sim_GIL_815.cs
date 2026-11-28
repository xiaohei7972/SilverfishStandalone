using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 恶毒的银行家
// Baleful Banker
// 战吼：选择一个友方随从，将一个复制洗入你的牌库。
// Battlecry: Choose a friendly minion. Shuffle a copy into your deck.
internal static class Sim_GIL_815
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
