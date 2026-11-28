using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 末日践行者
// Validated Doomsayer
// 在你的回合开始时，将本随从的攻击力 变为7。
// At the start of your turn, set this minion's Attack to 7.
[CardSim("OG_200")]
internal static class Sim_OG_200
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnStarted, TriggerRelation.Friendly, [Sim_OG_200e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
