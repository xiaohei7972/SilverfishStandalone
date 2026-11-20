namespace Silverfish.Simulation.Cards;

// 法术反制
// Counterspell
// 奥秘：当你的对手施放一个法术时，反制该法术。
// Secret: When your opponent casts a spell, Counter it.
[CardSim("EX1_287")]
internal static class Sim_EX1_287
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.SpellCounterWindow, TriggerRelation.Opponent, [new CounterSpellEffect()])
        ]
    };
}
