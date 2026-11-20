using System.Collections.Immutable;
using Silverfish.Simulation.Cards;

namespace Silverfish.Simulation;

/// <summary>将独立编写的卡牌规则与对应构建的静态属性绑定。</summary>
public static class StarterCardCatalog
{
    private static readonly Lazy<ImmutableArray<SimulationCard>> BuiltInCards = new(() =>
    {
        using var stream = typeof(StarterCardCatalog).Assembly.GetManifestResourceStream("Silverfish.Simulation.StarterCardData.json")
            ?? throw new InvalidOperationException("缺少内置 starter 卡牌数据。");
        return FromCardData(CardDataCatalog.LoadJson(stream));
    });

    public static ImmutableArray<SimulationCard> Cards => BuiltInCards.Value;

    public static ImmutableArray<SimulationCard> FromCardData(CardDataCatalog catalog) =>
        [.. Registered.Select(entry => SimulationCard.FromMetadata(catalog.GetCard(entry.CardId), entry.Rules))];

    private static (string CardId, CardRules Rules)[] Registered =>
    [
        (Sim_CS2_231.CardId, Sim_CS2_231.Rules),
        (Sim_CS2_172.CardId, Sim_CS2_172.Rules),
        (Sim_CS2_023.CardId, Sim_CS2_023.Rules),
        (Sim_CS2_032.CardId, Sim_CS2_032.Rules),
        (Sim_EX1_007.CardId, Sim_EX1_007.Rules),
        (Sim_EX1_402.CardId, Sim_EX1_402.Rules),
        (Sim_CS2_122.CardId, Sim_CS2_122.Rules),
        (Sim_CS2_222.CardId, Sim_CS2_222.Rules),
        (Sim_EX1_162.CardId, Sim_EX1_162.Rules),
        (Sim_CS2_092.CardId, Sim_CS2_092.Rules),
        (Sim_CS2_046.CardId, Sim_CS2_046.Rules),
        (Sim_EX1_360.CardId, Sim_EX1_360.Rules),
        (Sim_EX1_619.CardId, Sim_EX1_619.Rules),
        (Sim_EX1_019.CardId, Sim_EX1_019.Rules),
        (Sim_EX1_332.CardId, Sim_EX1_332.Rules),
        (Sim_EX1_245.CardId, Sim_EX1_245.Rules),
        (Sim_CFM_851.CardId, Sim_CFM_851.Rules),
        (Sim_CS2_106.CardId, Sim_CS2_106.Rules),
        (Sim_CS2_091.CardId, Sim_CS2_091.Rules),
        (Sim_CS2_112.CardId, Sim_CS2_112.Rules),
        (Sim_CS2_082.CardId, Sim_CS2_082.Rules),
        (Sim_CS2_097.CardId, Sim_CS2_097.Rules),
        (Sim_FP1_021.CardId, Sim_FP1_021.Rules),
        (Sim_EX1_066.CardId, Sim_EX1_066.Rules),
        (Sim_HERO_01bp.CardId, Sim_HERO_01bp.Rules),
        (Sim_HERO_03bp.CardId, Sim_HERO_03bp.Rules),
        (Sim_HERO_08bp.CardId, Sim_HERO_08bp.Rules),
        (Sim_HERO_06bp.CardId, Sim_HERO_06bp.Rules),
        (Sim_CS2_105.CardId, Sim_CS2_105.Rules),
        (Sim_EX1_581.CardId, Sim_EX1_581.Rules),
        (Sim_EX1_144.CardId, Sim_EX1_144.Rules),
        (Sim_EX1_049.CardId, Sim_EX1_049.Rules),
        (Sim_CS2_022.CardId, Sim_CS2_022.Rules),
        (Sim_CS2_tk1.CardId, Sim_CS2_tk1.Rules),
        (Sim_EX1_246.CardId, Sim_EX1_246.Rules),
        (Sim_hexfrog.CardId, Sim_hexfrog.Rules),
        (Sim_CS2_076.CardId, Sim_CS2_076.Rules),
        (Sim_CS1_113.CardId, Sim_CS1_113.Rules),
        (Sim_BRM_017.CardId, Sim_BRM_017.Rules),
        (Sim_CS2_024.CardId, Sim_CS2_024.Rules),
        (Sim_CS2_026.CardId, Sim_CS2_026.Rules),
        (Sim_CS2_028.CardId, Sim_CS2_028.Rules),
        (Sim_UNG_205.CardId, Sim_UNG_205.Rules),
        (Sim_CS2_033.CardId, Sim_CS2_033.Rules),
        (Sim_GIL_803.CardId, Sim_GIL_803.Rules),
        (Sim_CS2_124.CardId, Sim_CS2_124.Rules),
        (Sim_CS2_169.CardId, Sim_CS2_169.Rules),
        (Sim_EX1_033.CardId, Sim_EX1_033.Rules),
        (Sim_EX1_567.CardId, Sim_EX1_567.Rules),
        (Sim_EX1_238.CardId, Sim_EX1_238.Rules),
        (Sim_EX1_241.CardId, Sim_EX1_241.Rules),
        (Sim_EX1_259.CardId, Sim_EX1_259.Rules),
        (Sim_BRM_011.CardId, Sim_BRM_011.Rules),
        (Sim_OG_026.CardId, Sim_OG_026.Rules),
        (Sim_EX1_287.CardId, Sim_EX1_287.Rules),
        (Sim_EX1_610.CardId, Sim_EX1_610.Rules),
        (Sim_EX1_611.CardId, Sim_EX1_611.Rules),
        (Sim_EX1_609.CardId, Sim_EX1_609.Rules),
        (Sim_EX1_130.CardId, Sim_EX1_130.Rules),
        (Sim_EX1_130a.CardId, Sim_EX1_130a.Rules),
        (Sim_EX1_289.CardId, Sim_EX1_289.Rules),
        (Sim_EX1_554.CardId, Sim_EX1_554.Rules),
        (Sim_EX1_554t.CardId, Sim_EX1_554t.Rules),
        (Sim_EX1_277.CardId, Sim_EX1_277.Rules),
        (Sim_CS2_114.CardId, Sim_CS2_114.Rules),
        (Sim_EX1_251.CardId, Sim_EX1_251.Rules),
        (Sim_EX1_617.CardId, Sim_EX1_617.Rules),
        (Sim_EX1_407.CardId, Sim_EX1_407.Rules),
        (Sim_NEW1_031.CardId, Sim_NEW1_031.Rules),
        (Sim_NEW1_032.CardId, Sim_NEW1_032.Rules),
        (Sim_NEW1_033.CardId, Sim_NEW1_033.Rules),
        (Sim_NEW1_034.CardId, Sim_NEW1_034.Rules),
        (Sim_NEW1_019.CardId, Sim_NEW1_019.Rules),
        (Sim_EX1_082.CardId, Sim_EX1_082.Rules),
        (Sim_GVG_090.CardId, Sim_GVG_090.Rules),
        (Sim_GAME_005.CardId, Sim_GAME_005.Rules),
        (Sim_EX1_169.CardId, Sim_EX1_169.Rules),
        (Sim_SCH_427.CardId, Sim_SCH_427.Rules),
        (Sim_CS2_013.CardId, Sim_CS2_013.Rules),
        (Sim_CS2_013t.CardId, Sim_CS2_013t.Rules),
        (Sim_BT_130.CardId, Sim_BT_130.Rules),
        (Sim_BOT_054.CardId, Sim_BOT_054.Rules),
        (Sim_AT_038.CardId, Sim_AT_038.Rules),
        (Sim_LOOT_351.CardId, Sim_LOOT_351.Rules),
        (Sim_EX1_301.CardId, Sim_EX1_301.Rules),
        (Sim_LOOT_392.CardId, Sim_LOOT_392.Rules),
        (Sim_EX1_164.CardId, Sim_EX1_164.Rules),
        (Sim_WON_014.CardId, Sim_WON_014.Rules),
        (Sim_CFM_308.CardId, Sim_CFM_308.Rules)
    ];
}
