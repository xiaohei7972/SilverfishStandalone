using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 恐怖变形
// Dire Shapeshift
// 英雄技能 本回合+$a2攻击力。+$d2护甲值。
// Hero Power +$a2 Attack this turn. +$d2 Armor.
[CardSim("HERO_06bp2")]
internal static class Sim_HERO_06bp2
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            Sim_AT_132_DRUIDe.Rule.ApplyTo(StatTarget.FriendlyHero),
            new GainArmorEffect(2),
        ],
    };
}
