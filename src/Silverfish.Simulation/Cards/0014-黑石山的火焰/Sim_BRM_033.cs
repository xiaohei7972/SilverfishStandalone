using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 黑翼技师
// Blackwing Technician
// 战吼：如果你的手牌中有龙牌，便获得+1/+1。
// Battlecry: If you're holding a Dragon, gain +1/+1.
[CardSim("BRM_033")]
internal static class Sim_BRM_033
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
                [Sim_BRM_033e.Rule.ApplyTo(StatTarget.Source)])
        ]
    };
}
