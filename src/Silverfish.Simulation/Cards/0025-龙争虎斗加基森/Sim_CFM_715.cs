namespace Silverfish.Simulation.Cards;

// 青玉之灵
// Jade Spirit
// 战吼：召唤一个{0}的青玉魔像。@战吼：召唤一个 青玉魔像。
// Battlecry: Summon a{1} {0} Jade_Golem.@Battlecry: Summon a Jade_Golem.
[CardSim("CFM_715")]
internal static class Sim_CFM_715
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonJadeGolemEffect()],
    };
}
