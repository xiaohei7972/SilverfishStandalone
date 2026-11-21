namespace Silverfish.Simulation.Cards;

// 全副武装！
// Armor Up!
// 英雄技能 获得$d2点护甲值。
// Hero Power Gain $d2 Armor.
[CardSim("HERO_01bp")]
internal static class Sim_HERO_01bp
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [new GainArmorEffect(2)]
    };
}
