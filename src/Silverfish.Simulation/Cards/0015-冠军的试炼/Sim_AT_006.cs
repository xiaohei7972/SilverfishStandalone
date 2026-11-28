using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 达拉然铁骑士
// Dalaran Aspirant
// 法术伤害+1 激励：获得法术伤害+1。
// Spell Damage +1 Inspire: Gain Spell Damage +1.
[CardSim("AT_006")]
internal static class Sim_AT_006
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_006e.Rule.ApplyTo(StatTarget.Source)]),
        ],
        SpellDamage = 1,
    };
}
