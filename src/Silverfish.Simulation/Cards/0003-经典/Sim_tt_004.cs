using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 腐肉食尸鬼
// Flesheating Ghoul
// 每当一个随从死亡，便获得+1攻击力。
// Whenever a minion dies, gain +1 Attack.
[CardSim("tt_004")]
internal static class Sim_tt_004
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.MinionDied, TriggerRelation.Any, [Sim_tt_004o.Rule.ApplyTo(StatTarget.Source)])]
    };
}
