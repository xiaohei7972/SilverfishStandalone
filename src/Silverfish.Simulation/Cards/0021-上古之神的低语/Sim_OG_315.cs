using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 血帆教徒
// Bloodsail Cultist
// 战吼：如果你控制着其他海盗，使你的武器获得+1/+1。
// Battlecry: If you control another Pirate, give your weapon +1/+1.
[CardSim("OG_315")]
internal static class Sim_OG_315
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { MinionType = MinionType.Pirate }, ExcludeSource = true }, [Sim_OG_315e.Rule.ApplyToWeapon()]),
        ],
    };
}
