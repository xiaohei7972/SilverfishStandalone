using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 恶魔之心
// Demonheart
// 对一个随从造成$5点伤害，如果该随从是友方恶魔，则改为使其获得+5/+5。
// Deal $5 damage to a minion. If it's a friendly Demon, give it +5/+5 instead.
[CardSim("GVG_019")]
internal static class Sim_GVG_019
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
            new ConditionalEffect(
                new CharacterMatchCondition(
                    ConditionCharacter.Selected,
                    TriggerRelation.Friendly,
                    new CardFilter { MinionType = MinionType.Demon }),
                [
                    new EnchantStatsEffect(StatTarget.Selected, new(StatModifierMode.Add, 5, 5))
                    {
                        EnchantmentCardId = Sim_GVG_019e.CardId
                    }
                ])
            {
                Otherwise = [new DamageEffect(DamageTarget.Selected, 5)]
            }
        ]
    };
}
