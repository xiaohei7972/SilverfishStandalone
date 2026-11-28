using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 走私商贩
// Shady Dealer
// 战吼：如果你控制任何海盗，便获得+1/+1。
// Battlecry: If you have a Pirate, gain +1/+1.
[CardSim("AT_032")]
internal static class Sim_AT_032
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { MinionType = MinionType.Pirate }, ExcludeSource = true }, [Sim_AT_032e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
