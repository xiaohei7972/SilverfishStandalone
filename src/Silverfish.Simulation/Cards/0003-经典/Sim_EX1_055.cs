using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 魔瘾者
// Mana Addict
// 在本回合中，每当你施放一个法术，便获得+2攻击力。
// Whenever you cast a spell, gain +2 Attack this turn.
[CardSim("EX1_055")]
internal static class Sim_EX1_055
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [Sim_EX1_055o.Rule.ApplyTo(StatTarget.Source)])
        ]
    };
}
