namespace Silverfish.Simulation.Cards;

// 青玉酋长
// Jade Chieftain
// 战吼：召唤一个{0}的青玉魔像，使其获得嘲讽。@战吼：召唤一个青玉魔像，使其获得 嘲讽。
// Battlecry: Summon a{1} {0} Jade Golem. Give it Taunt. @Battlecry: Summon a Jade Golem. Give it Taunt.
[CardSim("CFM_312")]
internal static class Sim_CFM_312
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonJadeGolemEffect(Taunt: true)],
    };
}
