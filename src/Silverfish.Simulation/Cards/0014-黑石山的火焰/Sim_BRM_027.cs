namespace Silverfish.Simulation.Cards;

// 管理者埃克索图斯
// Majordomo Executus
// 亡语：用炎魔之王拉格纳罗斯替换你的英雄。
// Deathrattle: Replace your hero with Ragnaros the Firelord.
[CardSim("BRM_027")]
internal static class Sim_BRM_027
{
    internal static CardRules Rules { get; } = new()
    {
        Deathrattle =
        [
            new ReplaceHeroEffect("BRM_027h", "BRM_027p")
        ]
    };
}
