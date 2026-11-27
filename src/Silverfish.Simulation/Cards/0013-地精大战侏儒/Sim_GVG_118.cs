namespace Silverfish.Simulation.Cards;

// 颤地者特罗格佐尔
// Troggzor the Earthinator
// 每当你的对手施放一个法术，召唤一个石腭穴居人壮汉。
// Whenever your opponent casts a spell, summon a Burly Rockjaw Trogg.
[CardSim("GVG_118")]
internal static class Sim_GVG_118
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Opponent, [new SummonEffect("GVG_068")])
        ]
    };
}
