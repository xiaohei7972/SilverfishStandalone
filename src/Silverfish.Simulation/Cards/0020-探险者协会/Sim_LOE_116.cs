using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 遗物搜寻者
// Reliquary Seeker
// 战吼：如果你拥有六个其他随从，便获得+4/+4。
// Battlecry: If you have 6 other minions, gain +4/+4.
[CardSim("LOE_116")]
internal static class Sim_LOE_116
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ZoneCountCondition(ConditionPlayer.Friendly, GameZone.Play, NumberComparison.AtLeast, 6, EntityKind.Minion) { ExcludeSource = true }, [new EnchantStatsEffect(StatTarget.Source, new StatModifier(StatModifierMode.Add, 4, 4))]),
        ],
    };
}
