using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 化石魔暴龙
// Fossilized Devilsaur
// 战吼： 如果你控制着其他野兽，获得嘲讽。
// Battlecry: If you control another Beast, gain Taunt.
[CardSim("LOE_073")]
internal static class Sim_LOE_073
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.AtLeast, 1, EntityKind.Minion) { CardFilter = new CardFilter { MinionType = MinionType.Beast }, ExcludeSource = true }, [Sim_LOE_073e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
