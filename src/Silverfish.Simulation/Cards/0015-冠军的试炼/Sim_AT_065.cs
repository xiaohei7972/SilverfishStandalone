using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 国王护卫者
// King's Defender
// 战吼：如果你控制任何具有嘲讽的随从，便获得+1耐久度。
// Battlecry: If you have a minion with Taunt, gain +1 Durability.
[CardSim("AT_065")]
internal static class Sim_AT_065
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { HasTaunt = true } }, [Sim_AT_065e.Rule.ApplyToWeapon() with { UseSource = true }]),
        ],
    };
}
