using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 恶魔之爪
// Demon Claws
// 英雄技能 在本回合中+$a1 攻击力。
// Hero Power +$a1 Attack this turn.
[CardSim("HERO_10bp")]
internal static class Sim_HERO_10bp
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new EnchantStatsEffect(StatTarget.FriendlyHero, new StatModifier(StatModifierMode.Add, Attack: 1), EnchantmentDuration.CurrentTurn),
        ],
    };
}
