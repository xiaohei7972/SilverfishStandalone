namespace Silverfish.Simulation.Cards;

// 青玉之爪
// Jade Claws
// 战吼：召唤一个{0}的青玉魔像。 过载：（1）@战吼：召唤一个青玉魔像。 过载：（1）
// Battlecry: Summon a{1} {0} Jade Golem. Overload: (1)@Battlecry: Summon a Jade Golem. Overload: (1)
[CardSim("CFM_717")]
internal static class Sim_CFM_717
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonJadeGolemEffect()],
    };
}
