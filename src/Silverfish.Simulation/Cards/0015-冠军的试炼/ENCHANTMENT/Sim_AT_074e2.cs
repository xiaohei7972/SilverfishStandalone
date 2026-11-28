using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards.Enchantments;

// 英勇圣印
// Seal of Champions
// +3攻击力并具有圣盾。
// +3 Attack and Divine Shield.
internal static class Sim_AT_074e2
{
    internal static StatEnchantmentRule Rule { get; } =
        new("AT_074e2", new(StatModifierMode.Add, Attack: 3))
        {
            GrantedTag = GAME_TAG.DIVINE_SHIELD,
        };
}
