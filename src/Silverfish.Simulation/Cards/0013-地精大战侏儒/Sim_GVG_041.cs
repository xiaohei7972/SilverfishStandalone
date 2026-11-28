using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 黑暗私语
// Dark Wispers
// 抉择： 召唤5个小精灵；或者使一个随从获得+5/+5和嘲讽。
// Choose One - Summon 5 Wisps; or Give_a minion +5/+5 and Taunt.
[CardSim("GVG_041")]
internal static class Sim_GVG_041
{
    internal static PlayReqSet PlayReqs { get; } = new([], []);

    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new(
                "GVG_041a",
                [
                    new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, 5, 5))
                    {
                        GrantedTag = GAME_TAG.TAUNT,
                        EnchantmentCardId = Sim_GVG_041c.CardId
                    }
                ])
            {
                PlayReqs = [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)]
            },
            new(
                "GVG_041b",
                [
                    new SummonEffect("CS2_231"),
                    new SummonEffect("CS2_231"),
                    new SummonEffect("CS2_231"),
                    new SummonEffect("CS2_231"),
                    new SummonEffect("CS2_231")
                ])
            {
                PlayReqs = [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)]
            }
        ]
    };
}
