using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暮光雏龙
// Twilight Whelp
// 战吼：如果你的手牌中有龙牌，便获得+2生命值。
// Battlecry: If you're holding a Dragon, gain +2 Health.
[CardSim("BRM_004")]
internal static class Sim_BRM_004
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(
                new ZoneCountCondition(
                    ConditionPlayer.Friendly,
                    GameZone.Hand,
                    NumberComparison.AtLeast,
                    1)
                {
                    CardFilter = new()
                    {
                        MinionType = MinionType.Dragon
                    }
                },
                [Sim_BRM_004e.Rule.ApplyTo(StatTarget.Source)])
        ]
    };
}
