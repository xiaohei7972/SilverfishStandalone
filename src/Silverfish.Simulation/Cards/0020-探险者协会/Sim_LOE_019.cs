using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 化石迅猛龙
// Unearthed Raptor
// 战吼：选择一个友方随从，获得其亡语的复制。
// Battlecry: Choose a friendly minion. Gain a copy of its_Deathrattle.
[CardSim("LOE_019")]
internal static class Sim_LOE_019
{
    internal static PlayReqSet PlayReqs { get; } = new([new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_TARGET_WITH_DEATHRATTLE)], []);

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CopyDeathrattleEffect(Sim_LOE_019e.CardId),
        ],
    };
}
