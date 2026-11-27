namespace Silverfish.Simulation.Cards;

// 复制
// Duplicate
// 奥秘：当一个友方随从死亡时，将两张该随从的复制置入你的手牌。
// Secret: When a friendly minion dies, put 2 copies of it into your hand.
[CardSim("FP1_018")]
internal static class Sim_FP1_018
{
    internal static CardRules Rules { get; } = new()
    {
        IsSecret = true,
        Triggers =
        [
            new(
                TriggerTiming.MinionDied,
                TriggerRelation.Friendly,
                [new CopyTriggerCardToHandEffect(2)])
        ]
    };
}
