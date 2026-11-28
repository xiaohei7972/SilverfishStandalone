using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 镜像实体
// Mirror Entity
// 奥秘：在你的对手使用一张随从牌后，召唤一个该随从的复制。
// Secret: After your opponent plays a minion, summon a copy of it.
[CardSim("EX1_294")]
internal static class Sim_EX1_294
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AfterMinionPlayed, TriggerRelation.Opponent, [new SummonCopyEffect(EffectTarget.TriggerSubject)])
            {
                EntityKind = EntityKind.Minion,
                RequireLivingEntity = true,
                RequiresBoardSpace = true
            }
        ]
    };
}
