using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 神圣试炼
// Sacred Trial
// 奥秘：在你的对手使用一张随从牌后，如果他控制至少三个其他随从，则将其消灭。
// Secret: After your opponent has at least 3 minions and plays another, destroy it.
[CardSim("LOE_027")]
internal static class Sim_LOE_027
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterMinionPlayed, TriggerRelation.Opponent, [new DestroyMinionEffect(EffectTarget.TriggerSubject)]) { Condition = new ZoneCountCondition(ConditionPlayer.Opponent, GameZone.Play, NumberComparison.AtLeast, 4, EntityKind.Minion) },
        ],
        IsSecret = true,
    };
}
