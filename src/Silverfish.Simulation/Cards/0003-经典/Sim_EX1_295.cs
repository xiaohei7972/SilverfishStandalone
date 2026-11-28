using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 寒冰屏障
// Ice Block
// 奥秘：当你的英雄将要承受致命伤害时，防止这些伤害，并使其在本回合中免疫。
// Secret: When your hero takes fatal damage, prevent it and become Immune this turn.
[CardSim("EX1_295")]
internal static class Sim_EX1_295
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.FatalDamageWindow, TriggerRelation.Friendly,
                [new PreventDamageEffect(), Sim_EX1_295o.Rule.ApplyTo(StatTarget.FriendlyHero)])
            {
                EntityKind = EntityKind.Hero
            }
        ]
    };
}
