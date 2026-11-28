namespace Silverfish.Simulation.Cards;

// 恩佐斯的触须
// Tentacle of N'Zoth
// 亡语：对所有随从造成1点伤害。
// Deathrattle: Deal 1 damage to all minions.
[CardSim("OG_151")]
internal static class Sim_OG_151
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new DamageEffect(DamageTarget.AllMinions, 1),
        ],
    };
}
