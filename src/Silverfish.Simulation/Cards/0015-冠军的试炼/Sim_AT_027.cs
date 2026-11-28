using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 威尔弗雷德·菲兹班
// Wilfred Fizzlebang
// 你通过英雄技能抽到的卡牌，其法力值消耗为（0）点。
// Cards you draw from your Hero Power cost (0).
[CardSim("AT_027")]
internal static class Sim_AT_027
{
    internal static CardRules Rules { get; } = new()
    {
        SetHeroPowerDrawCostsToZeroEnchantment = Sim_AT_027e.CardId,
    };
}
