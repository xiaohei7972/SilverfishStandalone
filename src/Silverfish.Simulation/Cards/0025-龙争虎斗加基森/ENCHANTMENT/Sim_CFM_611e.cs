using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 恶魔气息
// Demonic Draught
// +3攻击力。
// +3 Attack.
internal static class Sim_CFM_611e
{
    internal const string CardId = "CFM_611e";
    internal static StatEnchantmentRule Rule { get; } =
        new(CardId, new(StatModifierMode.Add, Attack: 3));
}
