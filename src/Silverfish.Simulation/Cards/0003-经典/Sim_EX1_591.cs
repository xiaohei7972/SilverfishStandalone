namespace Silverfish.Simulation.Cards;

// 奥金尼灵魂祭司
// Auchenai Soulpriest
// 你的恢复生命值的牌和技能改为造成等量的伤害。
// Your cards and powers that restore Health now deal damage instead.
[CardSim("EX1_591")]
internal static class Sim_EX1_591
{
    internal static CardRules Rules { get; } = new()
    {
        HealingDealsDamage = true
    };
}
