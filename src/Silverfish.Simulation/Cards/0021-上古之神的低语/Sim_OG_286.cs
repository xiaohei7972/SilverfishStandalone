using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暮光尊者
// Twilight Elder
// 在你的回合结束时，使你的克苏恩 获得+1/+1（无论它在哪里）。
// At the end of your turn, give your C'Thun +1/+1 (wherever it is).
[CardSim("OG_286")]
internal static class Sim_OG_286
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new EmpowerCardEffect("OG_280", Sim_OG_290e.Rule.Modifier, Sim_OG_290e.Rule.CardId)]),
        ],
    };
}
