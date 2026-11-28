namespace Silverfish.Simulation.Cards;

// 黑曜石毁灭者
// Obsidian Destroyer
// 在你的回合结束时，召唤一只1/1并具有嘲讽的甲虫。
// At the end of your turn, summon a 1/1 Scarab with Taunt.
[CardSim("LOE_009")]
internal static class Sim_LOE_009
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new SummonEffect("LOE_009t")]),
        ],
    };
}
