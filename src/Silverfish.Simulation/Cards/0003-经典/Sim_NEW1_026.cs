namespace Silverfish.Simulation.Cards;

// 紫罗兰教师
// Violet Teacher
// 每当你施放一个法术，召唤一个1/1的紫罗兰学徒。
// Whenever you cast a spell, summon a 1/1 Violet Apprentice.
[CardSim("NEW1_026")]
internal static class Sim_NEW1_026
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly, [new SummonEffect("NEW1_026t")])]
    };
}
