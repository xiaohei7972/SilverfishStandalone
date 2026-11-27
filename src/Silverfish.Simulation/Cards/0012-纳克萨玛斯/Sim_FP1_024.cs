namespace Silverfish.Simulation.Cards;

// 蹒跚的食尸鬼
// Unstable Ghoul
// 嘲讽，亡语：对所有随从造成1点伤害。
// Taunt. Deathrattle: Deal 1 damage to all minions.
[CardSim("FP1_024")]
internal static class Sim_FP1_024
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new DamageEffect(DamageTarget.AllMinions, 1)
        ]
    };
}
