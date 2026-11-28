using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 银色警卫
// Argent Watchman
// 无法攻击。 激励：在本回合中可正常进行攻击。
// Can't attack. Inspire: Can attack as normal this turn.
[CardSim("AT_109")]
internal static class Sim_AT_109
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_109e.Rule.ApplyTo(StatTarget.Source)]),
        ],
        AllowAttackEnchantmentCardId = Sim_AT_109e.Rule.CardId,
    };
}
