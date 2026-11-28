namespace Silverfish.Simulation.Cards;

// 远古造物之血
// Blood of The Ancient One
// 在你的回合结束时，如果你控制两个远古造物之血，则将其融合成远古造物。
// If you control two of these at the end of your turn, merge them into 'The Ancient One'.
[CardSim("OG_173")]
internal static class Sim_OG_173
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new MergeMatchingMinionsEffect("OG_173a", 2)]),
        ],
    };
}
