namespace Silverfish.Simulation.Cards;

// 邪恶的巫医
// Wicked Witchdoctor
// 每当你施放一个法术，随机召唤一个基础图腾。
// Whenever you cast a spell, summon a random basic_Totem.
[CardSim("KAR_021")]
internal static class Sim_KAR_021
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.BeforeSpellResolved, TriggerRelation.Friendly,
                [new SummonRandomMinionEffect(["NEW1_009", "CS2_050", "CS2_051", "CS2_052"]) { Placement = SummonPlacement.RightOfSource }]),
        ],
    };
}
