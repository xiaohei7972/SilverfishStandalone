using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 亚煞极印记
// Mark of Y'Shaarj
// 使一个随从获得+2/+2。 如果该随从是野兽，抽一张牌。
// Give a minion +2/+2. If it's a Beast, draw a card.
[CardSim("OG_048")]
internal static class Sim_OG_048
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
            Sim_OG_048e.Rule.ApplyTo(StatTarget.Selected),
            new ConditionalEffect(new CharacterMatchCondition(ConditionCharacter.Selected, CardFilter: new CardFilter { MinionType = MinionType.Beast }), [new DrawEffect(1)]),
        ],
    };
}
