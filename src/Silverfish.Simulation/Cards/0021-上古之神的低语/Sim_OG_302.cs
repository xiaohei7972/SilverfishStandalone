using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 渡魂者
// Usher of Souls
// 每当一个随从死亡，使你的克苏恩获得+1/+1（无论它在哪里）。
// Whenever a minion dies, give your C'Thun +1/+1 (wherever it is).
[CardSim("OG_302")]
internal static class Sim_OG_302
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.MinionDied, TriggerRelation.Any, [new EmpowerCardEffect("OG_280", Sim_OG_302e.Rule.Modifier, Sim_OG_302e.Rule.CardId)]),
        ],
    };
}
