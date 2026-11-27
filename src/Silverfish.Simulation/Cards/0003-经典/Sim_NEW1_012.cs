using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 法力浮龙
// Mana Wyrm
// 每当你施放一个法术，便获得 +1攻击力。
// Whenever you cast a spell, gain +1 Attack.
[CardSim("NEW1_012")]
internal static class Sim_NEW1_012
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [Sim_NEW1_012o.Rule.ApplyTo(StatTarget.Source)])]
    };
}
