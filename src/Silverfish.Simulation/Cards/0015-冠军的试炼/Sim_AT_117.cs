using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 庆典司仪
// Master of Ceremonies
// 战吼：如果你控制任何具有法术伤害的随从，便获得+2/+2。
// Battlecry: If you have a minion with Spell Damage, gain +2/+2.
[CardSim("AT_117")]
internal static class Sim_AT_117
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { HasSpellDamage = true } }, [Sim_AT_117e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
