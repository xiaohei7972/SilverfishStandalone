using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 恶魔之咬
// Demon's Bite
// 英雄技能 在本回合中+$a2 攻击力。
// Hero Power +$a2 Attack this turn.
[CardSim("HERO_10bp2")]
internal static class Sim_HERO_10bp2
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.FriendlyHero, new StatModifier(StatModifierMode.Add, Attack: 2), EnchantmentDuration.CurrentTurn),
        ],
    };
}
