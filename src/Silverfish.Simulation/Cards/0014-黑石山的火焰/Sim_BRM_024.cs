using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 龙人打击者
// Drakonid Crusher
// 战吼：如果你对手的生命值小于或等于15点，便获得+3/+3。
// Battlecry: If your opponent has 15 or less Health, gain +3/+3.
[CardSim("BRM_024")]
internal static class Sim_BRM_024
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(
                new CharacterCondition(
                    ConditionCharacter.EnemyHero,
                    CharacterMetric.Health,
                    NumberComparison.AtMost,
                    15),
                [Sim_BRM_024e.Rule.ApplyTo(StatTarget.Source)])
        ]
    };
}
