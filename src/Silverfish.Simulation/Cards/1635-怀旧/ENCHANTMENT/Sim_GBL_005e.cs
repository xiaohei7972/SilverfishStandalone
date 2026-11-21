namespace Silverfish.Simulation.Cards.Enchantments;

// 卡名未收录（GBL_005e）
// Cost + 2
// 中文卡面描述未收录。
// Costs (2) more.
internal static class Sim_GBL_005e
{
    internal const string CardId = "GBL_005e";

    internal static CostEnchantmentRule Rule { get; } = new(CardId, 2);
}
