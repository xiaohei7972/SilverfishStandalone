using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 兜帽侍僧
// Hooded Acolyte
// 嘲讽。每当一个角色获得治疗时，使你的克苏恩获得+1/+1（无论它在哪里）。
// Taunt Whenever a character is healed, give your C'Thun +1/+1 (wherever it is).
[CardSim("OG_334")]
internal static class Sim_OG_334
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.Healed, TriggerRelation.Any, [new EmpowerCardEffect("OG_280", new StatModifier(StatModifierMode.Add, 1, 1), Sim_OG_281e.Rule.CardId)]),
        ],
    };
}
