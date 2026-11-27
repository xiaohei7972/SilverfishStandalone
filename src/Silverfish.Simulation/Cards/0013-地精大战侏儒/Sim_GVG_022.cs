using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 修补匠的磨刀油
// Tinker's Sharpsword Oil
// 使你的武器获得+3攻击力。连击：随机使一个友方随从获得+3攻击力。
// Give your weapon +3 Attack. Combo: Give a random friendly minion +3 Attack.
[CardSim("GVG_022")]
internal static class Sim_GVG_022
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ModifyWeaponEffect(AttackDelta: 3, EnchantmentCardId: Sim_GVG_022a.CardId),
            new ConditionalEffect(
                new ComboCondition(),
                [
                    new EnchantStatsEffect(StatTarget.RandomOtherFriendlyMinion, new(StatModifierMode.Add, 3))
                    {
                        EnchantmentCardId = Sim_GVG_022b.CardId
                    }
                ])
        ]
    };
}
