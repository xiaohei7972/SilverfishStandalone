namespace Silverfish.Simulation.Cards;

// 西风灯神
// Djinni of Zephyrs
// 在你对一个其他友方随从施放法术后，将法术效果复制在本随从身上。
// After you cast a spell on another friendly minion, cast a copy of it on this one.
[CardSim("LOE_053")]
internal static class Sim_LOE_053
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterSpellResolved, TriggerRelation.Friendly, [new CopySpellOntoSelfEffect()]) { SpellTargetOnOtherFriendlyMinion = true },
        ],
    };
}
