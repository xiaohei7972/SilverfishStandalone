namespace Silverfish.Simulation.Cards;

// 征募官
// Recruiter
// 激励：将一个2/2的侍从置入你的手牌。
// Inspire: Add a 2/2 Squire to your hand.
[CardSim("AT_113")]
internal static class Sim_AT_113
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new CreateCardEffect("CS2_152", Count: 1)]),
        ],
    };
}
