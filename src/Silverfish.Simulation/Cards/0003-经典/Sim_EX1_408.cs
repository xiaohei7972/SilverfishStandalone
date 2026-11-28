using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 致死打击
// Mortal Strike
// 造成$4点伤害；如果你的生命值小于或等于12点，则改为造成$6点伤害。
// Deal $4 damage. If you have 12 or less Health, deal $6 instead.
[CardSim("EX1_408")]
internal static class Sim_EX1_408
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(
                new CharacterCondition(ConditionCharacter.FriendlyHero, CharacterMetric.Health, NumberComparison.AtMost, 12),
                [new DamageEffect(DamageTarget.Selected, 6)])
            {
                Otherwise = [new DamageEffect(DamageTarget.Selected, 4)]
            }
        ]
    };
}
