using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 变形
// Shapeshift
// 英雄技能 本回合+$a1攻击力。+$d1护甲值。
// Hero Power +$a1 Attack this turn. +$d1 Armor.
[CardSim("HERO_06bp")]
internal static class Sim_HERO_06bp
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_CS2_017o.Rule.ApplyTo(StatTarget.FriendlyHero), new GainArmorEffect(1)]
    };
}
