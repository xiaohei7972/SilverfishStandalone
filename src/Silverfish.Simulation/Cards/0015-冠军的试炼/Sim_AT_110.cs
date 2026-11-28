namespace Silverfish.Simulation.Cards;

// 角斗场主管
// Coliseum Manager
// 激励：将本随从移回你的手牌。
// Inspire: Return this minion to your hand.
[CardSim("AT_110")]
internal static class Sim_AT_110
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [new ReturnMinionEffect(Target: EffectTarget.Source)]),
        ],
    };
}
