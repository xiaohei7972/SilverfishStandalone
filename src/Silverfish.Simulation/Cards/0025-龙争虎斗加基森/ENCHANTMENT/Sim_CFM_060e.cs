using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 法力强攻
// Mana Heist
// 攻击力提高。
// Increased Attack.
internal static class Sim_CFM_060e
{
    internal const string CardId = "CFM_060e";
    internal static StatEnchantmentRule Rule { get; } =
        new(CardId, new(StatModifierMode.Add, Attack: 2));
}
