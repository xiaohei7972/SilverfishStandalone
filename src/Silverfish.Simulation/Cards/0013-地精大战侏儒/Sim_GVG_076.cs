namespace Silverfish.Simulation.Cards;

// 自爆绵羊
// Explosive Sheep
// 亡语：对所有随从造成2点伤害。
// Deathrattle: Deal 2 damage to all minions.
[CardSim("GVG_076")]
internal static class Sim_GVG_076
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new DamageEffect(DamageTarget.AllMinions, 2)]
    };
}
