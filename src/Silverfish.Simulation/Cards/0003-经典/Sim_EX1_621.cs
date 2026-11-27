namespace Silverfish.Simulation.Cards;

// 治疗之环
// Circle of Healing
// 为所有随从恢复#4点生命值。
// Restore #4 Health to ALL_minions.
[CardSim("EX1_621")]
internal static class Sim_EX1_621
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new HealEffect(HealTarget.AllMinions, 4)]
    };
}
