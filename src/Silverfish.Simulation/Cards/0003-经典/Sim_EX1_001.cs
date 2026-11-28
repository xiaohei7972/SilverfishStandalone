using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 圣光护卫者
// Lightwarden
// 每当一个角色获得治疗，便获得 +2攻击力。
// Whenever a character is healed, gain +2 Attack.
[CardSim("EX1_001")]
internal static class Sim_EX1_001
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.Healed, TriggerRelation.Any, [Sim_EX1_001e.Rule.ApplyTo(StatTarget.Source)])]
    };
}
