using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 熔火怒犬
// Core Rager
// 战吼：如果你没有其他手牌，则获得+3/+3。
// Battlecry: If your hand is empty, gain +3/+3.
[CardSim("BRM_014")]
internal static class Sim_BRM_014
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(
                new ZoneCountCondition(
                    ConditionPlayer.Friendly,
                    GameZone.Hand,
                    NumberComparison.Equal,
                    0),
                [Sim_BRM_014e.Rule.ApplyTo(StatTarget.Source)])
        ]
    };
}
