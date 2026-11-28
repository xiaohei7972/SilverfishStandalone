namespace Silverfish.Simulation.Cards;

// 豹子戏法
// Cat Trick
// 奥秘：在你的对手施放一个法术后，召唤一只4/2并具有潜行的猎豹。
// Secret: After your opponent casts a spell, summon a 4/2 Panther with Stealth.
[CardSim("KAR_004")]
internal static class Sim_KAR_004
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(TriggerTiming.AfterSpellResolved, TriggerRelation.Opponent, [new SummonEffect("KAR_004a")]) { RequiresBoardSpace = true },
        ],
    };
}
