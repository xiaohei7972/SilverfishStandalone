using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 酸喉
// Acidmaw
// 每当有敌方随从受到伤害时，将其消灭。
// Whenever an enemy minion takes damage, destroy it.
[CardSim("AT_063")]
internal static class Sim_AT_063
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.Damage, TriggerRelation.Opponent, [new DestroyMinionEffect(EffectTarget.TriggerSubject)]) { EntityKind = EntityKind.Minion },
        ],
    };
}
