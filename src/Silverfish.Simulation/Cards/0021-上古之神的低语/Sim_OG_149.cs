namespace Silverfish.Simulation.Cards;

// 暴虐食尸鬼
// Ravaging Ghoul
// 战吼：对所有其他随从造成1点伤害。
// Battlecry: Deal 1 damage to all other minions.
[CardSim("OG_149")]
internal static class Sim_OG_149
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new DamageEffect(DamageTarget.OtherMinions, 1),
        ],
    };
}
