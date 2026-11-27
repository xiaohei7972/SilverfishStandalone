using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 维伦的恩泽
// Velen's Chosen
// 使一个随从获得+2/+4和法术伤害+1。
// Give a minion +2/+4 and Spell Damage +1.
[CardSim("GVG_010")]
internal static class Sim_GVG_010
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, 2, 4))
            {
                GrantedTag = GAME_TAG.SPELLPOWER,
                EnchantmentCardId = Sim_GVG_010b.CardId
            }
        ]
    };
}
