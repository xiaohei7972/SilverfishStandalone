namespace Silverfish.Simulation.Cards;

// 青玉巨兽
// Jade Behemoth
// 嘲讽，战吼：召唤一个{0}的青玉魔像。@嘲讽，战吼：召唤一个青玉魔像。
// Taunt Battlecry: Summon a{1} {0} Jade Golem.@Taunt Battlecry: Summon a Jade Golem.
[CardSim("CFM_343")]
internal static class Sim_CFM_343
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonJadeGolemEffect()],
    };
}
