namespace Silverfish.Simulation.Cards;

// 萨维斯
// Xavius
// 在你使用一张牌后，召唤一个2/1的萨特。
// After you play a card, summon a 2/1 Satyr.
[CardSim("EX1_614")]
internal static class Sim_EX1_614
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.AfterCardPlayed, TriggerRelation.Friendly, [new SummonEffect("EX1_614t")])]
    };
}
