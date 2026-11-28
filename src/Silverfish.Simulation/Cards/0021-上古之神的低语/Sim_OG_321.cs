using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 疯狂的信徒
// Crazed Worshipper
// 嘲讽。每当本随从受到伤害，使你的克苏恩获得+1/+1（无论它在哪里）。
// Taunt Whenever this minion takes damage, give your C'Thun +1/+1 (wherever it is).
[CardSim("OG_321")]
internal static class Sim_OG_321
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.Damage, TriggerRelation.Friendly, [new EmpowerCardEffect("OG_280", Sim_OG_321e.Rule.Modifier, Sim_OG_321e.Rule.CardId)]) { EntityMatch = TriggerEntityMatch.Self },
        ],
    };
}
