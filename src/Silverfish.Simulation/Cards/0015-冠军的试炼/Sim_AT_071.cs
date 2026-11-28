using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 阿莱克丝塔萨的勇士
// Alexstrasza's Champion
// 战吼：如果你的手牌中有龙牌，便获得+1攻击力和冲锋。
// Battlecry: If you're holding a Dragon, gain +1 Attack and Charge.
[CardSim("AT_071")]
internal static class Sim_AT_071
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Hand, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { MinionType = MinionType.Dragon } }, [Sim_AT_071e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
