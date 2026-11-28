namespace Silverfish.Simulation.Cards;

// 铜墙铁壁！
// Tank Up!
// 英雄技能 获得$d4点护甲值。
// Hero Power Gain $d4 Armor.
[CardSim("HERO_01bp2")]
internal static class Sim_HERO_01bp2
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new GainArmorEffect(4),
        ],
    };
}
