using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 谦逊
// Humility
// 攻击力变为1。
// Attack has been changed to 1.
internal static class Sim_EX1_360e
{
    internal const string CardId = "EX1_360e";

    internal static StatEnchantmentRule Rule { get; } = new(CardId,
        new(StatModifierMode.Set, Attack: 1));
}
