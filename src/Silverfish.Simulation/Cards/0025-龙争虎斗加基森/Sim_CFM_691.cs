namespace Silverfish.Simulation.Cards;

// 青玉游荡者
// Jade Swarmer
// 潜行，亡语：召唤一个{0}的青玉魔像。@潜行，亡语：召唤一个青玉魔像。
// Stealth Deathrattle: Summon a{1} {0} Jade Golem.@Stealth Deathrattle: Summon a Jade Golem.
[CardSim("CFM_691")]
internal static class Sim_CFM_691
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new SummonJadeGolemEffect()],
    };
}
