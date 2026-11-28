namespace Silverfish.Simulation.Cards.Enchantments;

// 卡名未收录（GBL_002e）
// Cost - 2
// 中文卡面描述未收录。
// Costs (2) less.
internal static class Sim_GBL_002e
{
    internal const string CardId = "GBL_002e";

    internal static CostEnchantmentRule Rule { get; } = new(CardId, -2);
}
