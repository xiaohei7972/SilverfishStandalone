using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暮光守护者
// Twilight Guardian
// 战吼：如果你的手牌中有龙牌，便获得+1攻击力和嘲讽。
// Battlecry: If you're holding a Dragon, gain +1 Attack and Taunt.
[CardSim("AT_017")]
internal static class Sim_AT_017
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Hand, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { MinionType = MinionType.Dragon } }, [Sim_AT_017e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
