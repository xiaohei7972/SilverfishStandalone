namespace Silverfish.Simulation.Cards;

// 死亡之咬
// Death's Bite
// 亡语：对所有随从造成1点伤害。
// Deathrattle: Deal 1 damage to all minions.
[CardSim("FP1_021")]
internal static class Sim_FP1_021
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle = [new DamageEffect(DamageTarget.AllMinions, 1)]
    };
}
