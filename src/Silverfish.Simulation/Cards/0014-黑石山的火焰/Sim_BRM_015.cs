namespace Silverfish.Simulation.Cards;

// 复仇打击
// Revenge
// 对所有随从造成1点伤害。如果你的生命值小于或等于12点，则改为造成3点伤害。
// Deal 1 damage to all minions. If you have 12 or less Health, deal 3 damage instead.
[CardSim("BRM_015")]
internal static class Sim_BRM_015
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(
                new CharacterCondition(
                    ConditionCharacter.FriendlyHero,
                    CharacterMetric.Health,
                    NumberComparison.AtMost,
                    12),
                [new DamageEffect(DamageTarget.AllMinions, 3)])
            {
                Otherwise =
                [
                    new DamageEffect(DamageTarget.AllMinions, 1)
                ]
            }
        ]
    };
}
