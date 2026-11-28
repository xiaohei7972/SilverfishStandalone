namespace Silverfish.Simulation.Cards;

// 治疗图腾
// Healing Totem
// 在你的回合结束时，为所有友方随从恢复#1点生命值。
// At the end of your turn, restore #1 Health to all friendly minions.
[CardSim("NEW1_009")]
internal static class Sim_NEW1_009
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TurnEnded, TriggerRelation.Friendly, [new HealEffect(HealTarget.FriendlyMinions, 1)]),
        ],
    };
}
