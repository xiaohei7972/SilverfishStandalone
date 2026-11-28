namespace Silverfish.Simulation.Cards;

// 裁决者图哈特
// Justicar Trueheart
// 战吼：以更强的英雄技能来替换你的初始英雄技能。
// Battlecry: Replace your starting Hero Power with a better one.
[CardSim("AT_132")]
internal static class Sim_AT_132
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new UpgradeHeroPowerEffect(
            [
                new("HERO_01bp", "HERO_01bp2"),
                new("HERO_02bp", "HERO_02bp2"),
                new("HERO_03bp", "HERO_03bp2"),
                new("HERO_04bp", "HERO_04bp2"),
                new("HERO_05bp", "HERO_05bp2"),
                new("HERO_06bp", "HERO_06bp2"),
                new("HERO_07bp", "HERO_07bp2"),
                new("HERO_08bp", "HERO_08bp2"),
                new("HERO_09bp", "HERO_09bp2"),
                new("HERO_10bp", "HERO_10bp2"),
                new("HERO_11bp", "HERO_11bp2"),
            ]),
        ],
    };
}
