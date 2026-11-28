using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暗影施法者
// Shadowcaster
// 战吼：选择一个友方随从，将它的一张1/1的复制置入你的手牌，其法力值消耗为（1）点。
// Battlecry: Choose a friendly minion. Add a 1/1 copy to_your hand that costs_(1).
[CardSim("OG_291")]
internal static class Sim_OG_291
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CopyTargetCardEffect(Placement: CardPlacement.Hand) { SetCost = 1, Modifier = Sim_OG_291e.Rule.Modifier, EnchantmentCardId = Sim_OG_291e.Rule.CardId },
        ],
    };
}
