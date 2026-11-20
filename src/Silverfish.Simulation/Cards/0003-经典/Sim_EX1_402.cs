using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 铸甲师
// Armorsmith
// 每当一个友方随从受到伤害，便获得1点护甲值。
// Whenever a friendly minion_takes damage, gain 1 Armor.
[CardSim("EX1_402")]
internal static class Sim_EX1_402
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.Damage, TriggerRelation.Friendly, [new GainArmorEffect(1)])
        {
            EntityKind = EntityKind.Minion
        }]
    };
}
