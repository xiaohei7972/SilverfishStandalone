using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 狙击
// Snipe
// 奥秘：在你的对手使用一张随从牌后，对该随从造成$6点伤害。
// Secret: After your opponent plays a minion, deal $6 damage to it.
[CardSim("EX1_609")]
internal static class Sim_EX1_609
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AfterMinionPlayed, TriggerRelation.Opponent, [new DamageEffect(DamageTarget.TriggerSubject, 6)])
            {
                EntityKind = EntityKind.Minion, RequireLivingEntity = true
            }
        ]
    };
}
