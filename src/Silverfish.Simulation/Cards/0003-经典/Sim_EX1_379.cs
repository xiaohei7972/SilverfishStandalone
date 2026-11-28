using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 忏悔
// Repentance
// 奥秘： 在你的对手使用一张随从牌后，使该随从的生命值降为1。
// Secret: After your opponent plays a minion, reduce its Health to 1.
[CardSim("EX1_379")]
internal static class Sim_EX1_379
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AfterMinionPlayed, TriggerRelation.Opponent, [Sim_EX1_379e.Rule.ApplyTo(StatTarget.TriggerSubject)])
            {
                EntityKind = EntityKind.Minion,
                RequireLivingEntity = true
            }
        ]
    };
}
