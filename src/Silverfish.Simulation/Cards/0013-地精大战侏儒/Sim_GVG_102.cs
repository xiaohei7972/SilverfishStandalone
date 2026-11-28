using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 工匠镇技师
// Tinkertown Technician
// 战吼：如果你控制一个机械，便获得+1/+1并将一张零件牌置入你的手牌。
// Battlecry: If you have a Mech, gain +1/+1 and add a Spare Part to your hand.
[CardSim("GVG_102")]
internal static class Sim_GVG_102
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(
                new ZoneCountCondition(
                    ConditionPlayer.Friendly,
                    GameZone.Play,
                    NumberComparison.AtLeast,
                    1,
                    EntityKind.Minion)
                {
                    CardFilter = new CardFilter { MinionType = MinionType.Mech }
                },
                [
                    new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, 1, 1))
                    {
                        EnchantmentCardId = Sim_GVG_102e.CardId
                    },
                    new CreateRandomCardEffect(SpareParts.All)
                ])
        ]
    };
}
