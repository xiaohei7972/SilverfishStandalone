using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 狂野争斗者
// Savage Combatant
// 激励：在本回合中，使你的英雄获得+2攻击力。
// Inspire: Give your hero +2 Attack this turn.
[CardSim("AT_039")]
internal static class Sim_AT_039
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_039e.Rule.ApplyTo(StatTarget.FriendlyHero)]),
        ],
    };
}
