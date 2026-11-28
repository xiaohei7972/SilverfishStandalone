namespace Silverfish.Simulation.Cards;

// 艾雅·黑掌
// Aya Blackpaw
// 战吼，亡语：召唤一个{0}的青玉魔像。@战吼，亡语：召唤一个青玉魔像。
// Battlecry and Deathrattle: Summon a{1} {0} Jade Golem. @ Battlecry and Deathrattle: Summon a Jade Golem.
[CardSim("CFM_902")]
internal static class Sim_CFM_902
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new SummonJadeGolemEffect()],
        Deathrattle = [new SummonJadeGolemEffect()],
    };
}
