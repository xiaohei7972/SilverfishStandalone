using Silverfish.Domain;

namespace Silverfish.Simulation.Cards.Enchantments;

// 恶魔气息
// Demonic Draught
// +3/+3。
// +3/+3.
internal static class Sim_CFM_611e2
{
    internal const string CardId = "CFM_611e2";
    internal static StatEnchantmentRule Rule { get; } =
        new(CardId, new(StatModifierMode.Add, Attack: 3, MaximumHealth: 3));
}
