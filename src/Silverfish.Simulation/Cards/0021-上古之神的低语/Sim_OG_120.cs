namespace Silverfish.Simulation.Cards;

// 阿诺玛鲁斯
// Anomalus
// 亡语：对所有随从造成8点伤害。
// Deathrattle: Deal 8 damage to all minions.
[CardSim("OG_120")]
internal static class Sim_OG_120
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new DamageEffect(DamageTarget.AllMinions, 8),
        ],
    };
}
