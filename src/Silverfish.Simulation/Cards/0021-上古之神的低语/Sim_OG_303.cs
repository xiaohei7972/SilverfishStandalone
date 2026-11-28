using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 异教女巫
// Cult Sorcerer
// 法术伤害+1 在你施放一个法术后，使你的克苏恩获得+1/+1（无论它在哪里）。
// Spell Damage +1 After you cast a spell, give your C'Thun +1/+1 (wherever it is).
[CardSim("OG_303")]
internal static class Sim_OG_303
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new EmpowerCardEffect("OG_280", Sim_OG_303e.Rule.Modifier, Sim_OG_303e.Rule.CardId)]),
        ],
        SpellDamage = 1,
    };
}
